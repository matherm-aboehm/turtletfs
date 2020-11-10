using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Ankh.ExtensionPoints.IssueTracker;
using Ankh.TfsProvider.Extension.IssueTracker.Forms;
using TurtleTfs;

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

		TfsProviderOptions _options;
		IDictionary<string, object> _properties;
		IssuesListView _control;

		public static IssueRepository Create(IssueRepositorySettings settings)
		{
			if (settings != null)
			{
				return new AnkhRepository(settings.RepositoryUri, settings.RepositoryId, settings.CustomProperties);
			}
			return null;
		}

		public AnkhRepository(Uri uri, string repositoryId, IDictionary<string, object> properties)
			: base(AnkhConnector.ConnectorName)
		{
			_options = new TfsProviderOptions();
			_options.ServerName = uri.ToString();
			_options.ProjectName = repositoryId;
			if (properties != null)
			{
				object value;
				if (properties.TryGetValue(PROPERTY_USERNAME, out value))
					_options.UserName = (string)value;
				if (properties.TryGetValue(PROPERTY_PASSWORD, out value))
					_options.UserPassword = (string)value;
				if (properties.TryGetValue(PROPERTY_VSO, out value))
					_options.VisualStudioOnline = (bool)value;
				_properties = properties;
			}
		}

		/// <summary>
		/// Gets the repository connection URL
		/// </summary>
		public override Uri RepositoryUri
		{
			get { return !Uri.TryCreate(_options.ServerName, UriKind.Absolute, out var tfsUri) ? tfsUri : null; }
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

		public override void PreCommit(PreCommitArgs args)
		{
			bool valid = true; // perform issue related pre-commit checks
			if (valid)
			{
				// modify commit message here
				string originalMessage = args.CommitMessage ?? string.Empty;
				// get _control.SelectedIssues
				// modify original message to include issue info in the message
				args.CommitMessage = originalMessage;
			}
			args.Cancel = valid; // true if "some" pre-commit check fails
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
		public override void NavigateTo(string issueId)
		{
			// show issue details
			if (!string.IsNullOrEmpty(issueId))
			{
				string message = string.Format("{0}/{1}/_workitems#_a=edit&id={2}", _options.ServerName, _options.ProjectName, issueId);
				MessageBox.Show(message, "Navigate to Issue", MessageBoxButtons.OK);
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

		internal IssuesListView Control
		{
			get
			{
				if (_control == null)
				{
					_control = CreateControl();
				}
				return _control;
			}
		}

		private IssuesListView CreateControl()
		{
			return new Forms.IssuesListView();
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
