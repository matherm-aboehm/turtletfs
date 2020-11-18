using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Windows.Forms;
using System.Windows.Threading;
using Ankh.Configuration;
using Ankh.ExtensionPoints.IssueTracker;
using Ankh.UI;
using Ankh.VS;
using Microsoft.TeamFoundation.Client;
using Microsoft.VisualStudio.Threading;
using TurtleTfs;
using TurtleTfs.Controls;

namespace Ankh.TfsProvider.Extension.IssueTracker
{
	/// <summary>
	/// Represents an Issue Repository
	/// </summary>
	internal class AnkhRepository : IssueRepository, IWin32Window, IDisposable
	{
		public const string PROPERTY_USERNAME = "username";
		public const string PROPERTY_PASSWORD = "password";
		public const string PROPERTY_VSO = "vso";

		private static readonly Regex _reIssueId = new Regex(Properties.Resources.IssueIdPattern, RegexOptions.CultureInvariant | RegexOptions.Compiled);

		readonly IAnkhPackage _package;
		TfsProviderOptions _options;
		IDictionary<string, object> _properties;
		IssuesBrowserControl _control;

		internal TfsProviderOptions Options { get => _options; }

		public static IssueRepository Create(IssueRepositorySettings settings, IAnkhPackage package)
		{
			if (settings != null)
			{
				TfsProviderOptions options = CreateTfsProviderOptions(settings);
				return new AnkhRepository(package, options, settings.CustomProperties);
			}
			return null;
		}

		internal static TfsProviderOptions CreateTfsProviderOptions(IssueRepositorySettings settings)
		{
			TfsProviderOptions options = new TfsProviderOptions();
			options.ServerName = settings.RepositoryUri?.ToString();
			options.ProjectName = settings.RepositoryId;
			var properties = settings.CustomProperties;
			if (properties != null)
			{
				object value;
				if (properties.TryGetValue(PROPERTY_USERNAME, out value))
					options.UserName = (string)value;
				if (properties.TryGetValue(PROPERTY_PASSWORD, out value))
					options.UserPassword = (string)value;
				if (properties.TryGetValue(PROPERTY_VSO, out value) &&
					bool.TryParse(value.ToString(), out bool vso))
					options.VisualStudioOnline = vso;
			}
			return options;
		}

		public AnkhRepository(IAnkhPackage package, TfsProviderOptions options, IDictionary<string, object> properties = null)
			: base(AnkhConnector.ConnectorName)
		{
			_package = package;
			_options = options;
			if (properties == null)
			{
				_properties = new Dictionary<string, object>();
				_properties.Add(PROPERTY_USERNAME, options.UserName);
				_properties.Add(PROPERTY_PASSWORD, options.UserPassword);
				_properties.Add(PROPERTY_VSO, options.VisualStudioOnline);
			}
			else
			{
				_properties = properties;
			}
		}

		/// <summary>
		/// Gets the repository connection URL
		/// </summary>
		public override Uri RepositoryUri
		{
			get { return Uri.TryCreate(_options.ServerName, UriKind.Absolute, out var tfsUri) ? tfsUri : null; }
		}

		/// <summary>
		/// Gets the repository id hosted on the RepositoryUri
		/// </summary>
		/// <remarks>optional</remarks>
		public override string RepositoryId
		{
			get { return _options.ProjectName; }
		}

		/// <summary>
		/// Gets the custom properties specific to this type of connector
		/// </summary>
		public override IDictionary<string, object> CustomProperties
		{
			get { return _properties; }
		}

		/// <summary>
		/// Gets the repository label
		/// </summary>
		public override string Label
		{
			get { return RepositoryId ?? (RepositoryUri == null ? string.Empty : RepositoryUri.ToString()); }
		}

		[Obsolete("Use " + nameof(IssueIdRegex))]
		public override string IssueIdPattern
		{
			get
			{
				// reg expression to recognize issue id's within a text (i.e commit log message)
				// for example:
				// Text -> Sample id001, #id002 and id003
				// Resolved Issue Ids -> id001, id002, id003
				// How to test: 
				// 1. Set the current Issue repository to be this.
				// 2. Type a commit message in Pending Changes message box that would match this pattern
				// 3. See that issue ids are colorized, and "open issue" context option is available
				//return @"[Ss]ample?:?\s*(#\s*)?(?<id>id\d+)(\s*(,|and)\s*(#\s*)?(?<id>id\d+))*";
				return Properties.Resources.IssueIdPattern;
			}
		}

		public override Regex IssueIdRegex => _reIssueId;

		public override void PreCommit(PreCommitArgs args)
		{
			bool valid = true; // perform issue related pre-commit checks
			if (valid)
			{
				// modify commit message here
				string originalMessage = args.CommitMessage ?? string.Empty;

				string issuesMessage = Control.AssociatedWorkItemsMessage;
				// modify original message to include issue info in the message
				if (!string.IsNullOrEmpty(issuesMessage))
				{
					if (!string.IsNullOrEmpty(originalMessage))
						originalMessage = originalMessage + Environment.NewLine + Environment.NewLine + issuesMessage;
					else
						originalMessage = issuesMessage;
				}
				args.CommitMessage = originalMessage;
			}
			args.Cancel = !valid; // true if "some" pre-commit check fails
		}

		public override void PostCommit(PostCommitArgs args)
		{
			// post-process selected issues after commit is done
			// i.e. change issue status, add a comment to the issue(s) about commit info etc.
			long committedRevision = args.Revision;
			string commitMessage = args.CommitMessage;

			base.PostCommit(args);
		}

		/// <summary>
		/// Show issue details
		/// </summary>
		/// <param name="issueId"></param>
		public override async void NavigateTo(string issueId)
		{
			// show issue details
			if (!string.IsNullOrEmpty(issueId))
			{
				int workItemId = int.Parse(issueId);
				string url;
				var cs = _package.GetService<IAnkhConfigurationService>();
				bool external = false;
				if (cs != null && cs.Instance.ForceExternalBrowser)
					external = true;

				if (external)
					url = IssuesBrowserControl.FormatWorkItemAddress(workItemId, _options);
				else
					url = $"vstfs:///WorkItemTracking/WorkItem/{workItemId}";
				//MessageBox.Show(url, "Navigate to Issue", MessageBoxButtons.OK);
				Uri uri;
				if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
					return;

				if (uri != null && !uri.IsFile && !uri.IsUnc)
				{
					bool openedInternal = false;
					if (!external)
					{
						var calledOpen = new TaskCompletionSource<bool>();
						async Task openWorkItem()
						{
							var tfsContextManager = await TfsProviderPackage.Instance.GetServiceAsync<ITeamFoundationContextManager5>();
							var tfsClientLinking = await TfsProviderPackage.Instance.GetServiceAsync<IClientLinking>();
							if (tfsContextManager != null && tfsClientLinking != null)
							{
								CancellationTokenSource cancelContextChange = new CancellationTokenSource();
								using (TfsProviderPackage.Instance.JoinableTaskContextNode.RegisterHangDetection(hd =>
								{
									if (hd.HangDuration >= TimeSpan.FromSeconds(30))
									{
										Trace.WriteLine("Opening the WorkItem took longer than 30 seconds, it possibly hangs, so this task will be canceled.");
										cancelContextChange.Cancel();
									}
								}))
								{
									TfsProviderPackage.Instance.JoinableTaskFactory.Run(async () =>
									{
										await new CurrentThreadTaskScheduler();
										using (var connectionContext = Control.AcquireConnectionContext())
										{
											var tfs = ((IServiceProvider)connectionContext).GetService<TfsTeamProjectCollection>();
											var tfsDisposing = ((IServiceProvider)connectionContext).GetService<ITfsConnectionDisposing>();
											await TfsProviderPackage.Instance.JoinableTaskFactory.RunAsync(async () =>
											{
												bool resetContext = false;
												var oldtfs = tfsContextManager.CurrentContext.TeamProjectCollection;
												var oldproject = tfsContextManager.CurrentContext.TeamProjectName;
												try
												{
													await TfsProviderPackage.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();
													await tfsContextManager.SetContextAsync(tfs, _options.ProjectName, Guid.Empty, null, true,
															ActiveContextChangeReason.DocumentOpen, true);
													//TODO: Currently the global context can't be reset immediately,
													//because the editor window reacts on it and just closes.
													//But the TFS connection that was used for the new context,
													//will be Disposed() when it isn't needed anymore.
													//So we need a "Dispose-protection", "remembering" or "reconnect" strategy
													//in either the editor window or here or both, before the context reset can be done.
													//For now register a callback in finally
													resetContext = oldtfs != tfs;
													tfsClientLinking.ExecuteDefaultAction(uri.ToString(), null /* repositoryId */);
													calledOpen.SetResult(true);
													await TaskExtensions.IdleYield();
												}
												catch (Exception ex)
												{
													calledOpen.SetException(ex);
												}
												finally
												{
													if (resetContext)
														tfsDisposing.Disposing += (s, e) => tfsContextManager.SetContext(oldtfs, oldproject, Guid.Empty, null, false,
																  ActiveContextChangeReason.DocumentOpen, true);
												}
											}).JoinAsync(cancelContextChange.Token);
											return;
										}
									});
								}
							}
						}

						try
						{
							//Switch to threadpool thread, so that ui thread does not deadlock when waiting for the
							//document to open.
							await TaskScheduler.Default;
							var openTask = TfsProviderPackage.Instance.JoinableTaskFactory.RunAsync(openWorkItem);
							var completedTask = await Task.WhenAny(calledOpen.Task, openTask.JoinAsync());
							if (completedTask is Task<bool> calledOpenTask)
								openedInternal = await calledOpenTask;
							else
								await completedTask; //only rethrow Exception, because openTask can't complete before calledOpen.Task
							await TfsProviderPackage.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();
							var catchExceptionTask = openTask.HandleExceptionOnCurrentThread(ex =>
							{
								// This is separately executed on the thread for the current SynchronizationContext
								// and not in current async context, so outer try/catch isn't working here.
								Trace.WriteLine(ex.ToString());
								MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
								return true; // was handled, don't rethrow
							});
						}
						catch (Exception ex)
						{
							await TfsProviderPackage.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();
							Trace.WriteLine(ex.ToString());
							MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					}

					if (!openedInternal)
					{
						var web = _package.GetService<IAnkhWebBrowser>();
						if (web != null)
							web.Navigate(uri);
					}
				}
			}
		}

		internal void SetProperty(string property, object value)
		{
			if (string.IsNullOrEmpty(property)) { return; }
			if (CustomProperties == null)
			{
				_properties = new Dictionary<string, object>();
			}
			_properties.Add(property, value);
			switch (property)
			{
				case PROPERTY_USERNAME:
					_options.UserName = (string)value;
					break;
				case PROPERTY_PASSWORD:
					_options.UserPassword = (string)value;
					break;
				case PROPERTY_VSO:
					_options.VisualStudioOnline = (bool)value;
					break;
				default:
					break;
			}
		}

		#region IWin32Window Members

		public IntPtr Handle
		{
			get { return Control.Handle; }
		}

		#endregion

		internal IssuesBrowserControl Control
		{
			get
			{
				if (_control == null)
				{
					_control = CreateControl();
					_control.OpenIssue += Control_OpenIssue;
				}
				return _control;
			}
		}

		private IssuesBrowserControl CreateControl()
		{
			return new IssuesBrowserControl() { Options = _options };
		}

		private void Control_OpenIssue(object sender, OpenIssueEventArgs e)
		{
			NavigateTo(e.IssueId.ToString());
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (_control != null && !_control.IsDisposed && !_control.Disposing)
			{
				_control.Dispose();
			}
			_control = null;
		}

		#endregion
	}
}
