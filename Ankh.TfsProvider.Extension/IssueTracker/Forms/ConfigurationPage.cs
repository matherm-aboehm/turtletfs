using System;
using System.Windows.Forms;
using Ankh.ExtensionPoints.IssueTracker;

namespace Ankh.TfsProvider.Extension.IssueTracker.Forms
{
	/// <summary>
	/// UI for Issue repository configuration
	/// </summary>
	public partial class ConfigurationPage : UserControl
	{
		IssueRepositorySettings _currentSettings;
		private bool _processedSettings = false;
		private bool _canProcessSettings = false;

		public ConfigurationPage()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			_canProcessSettings = true;
			SelectSettings();
		}

		internal IssueRepositorySettings Settings
		{
			get
			{
				return CreateRepositorySettings();
			}
			set
			{
				_currentSettings = value;
				SelectSettings();
			}
		}

		internal AnkhRepository CreateRepositorySettings()
		{
			AnkhRepository repo = null;
			Uri repoUri;
			string uriString = _url.Text.Trim();
			if (Uri.TryCreate(uriString, UriKind.Absolute, out repoUri))
			{
				repo = new AnkhRepository(repoUri, null, null);
				repo.SetProperty(AnkhRepository.PROPERTY_USERNAME, _user.Text.Trim());
				repo.SetProperty(AnkhRepository.PROPERTY_PASSWORD, _password.Text.Trim());
			}
			return repo;
		}

		/// <summary>
		/// populate UI with existing settings
		/// </summary>
		private void SelectSettings()
		{
			if (_currentSettings != null
				&& !_processedSettings
				&& _canProcessSettings)
			{
				try
				{
					_url.Text = _currentSettings.RepositoryUri.ToString();
					if (_currentSettings.CustomProperties != null)
					{
						object value;
						if (_currentSettings.CustomProperties.TryGetValue(AnkhRepository.PROPERTY_USERNAME, out value))
						{
							if (value != null)
							{
								_user.Text = value.ToString();
							}
						}
						value = null;
						if (_currentSettings.CustomProperties.TryGetValue(AnkhRepository.PROPERTY_PASSWORD, out value))
						{
							if (value != null)
							{
								_password.Text = value.ToString();
							}
						}
					}
				}
				finally
				{
					_processedSettings = true;
				}
			}
		}

		#region UI Events

		private void UrlModified(object sender, EventArgs e)
		{
			RaisePageEvent();
		}

		#endregion

		/// <summary>
		/// Raise an event to notify listeners about the user changes
		/// </summary>
		private void RaisePageEvent()
		{
			ConfigPageEventArgs args = new ConfigPageEventArgs();
			try
			{
				args.IsComplete = IsPageComplete;
			}
			catch (Exception exc)
			{
				args.IsComplete = false;
				args.Exception = exc;
			}
			if (OnPageEvent != null)
			{
				OnPageEvent(this, args);
			}
		}

		private bool IsPageComplete
		{
			get
			{
				string urlString = _url.Text.Trim();
				Uri uri;
				return Uri.TryCreate(urlString, UriKind.Absolute, out uri);
				// add additional checks if required
			}
		}

		public event EventHandler<ConfigPageEventArgs> OnPageEvent;
	}
}
