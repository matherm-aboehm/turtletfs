using System;
using System.Windows.Forms;
using Ankh.ExtensionPoints.IssueTracker;
using Ankh.TfsProvider.Extension.IssueTracker.Forms;

namespace Ankh.TfsProvider.Extension.IssueTracker
{
	/// <summary>
	/// Configuration page 
	/// </summary>
	class AnkhConfigurationPage : IssueRepositoryConfigurationPage, IWin32Window
	{
		IssueRepositorySettings _settings;

		public AnkhConfigurationPage()
		{ }

		/// <summary>
		/// Gets or Sets the config settings
		/// </summary>
		public override IssueRepositorySettings Settings
		{
			get
			{
				if (_control != null)
				{
					// reconcile old settings with new settings
					return Reconcile(_settings, _control.CreateRepositorySettings());
				}
				return null;
			}
			set
			{
				IssueRepositorySettings currentSettings = value;
				if (currentSettings != null
					&& (false
						|| (true
							&& string.Equals(currentSettings.ConnectorName, AnkhConnector.ConnectorName)
							)
						)
					)
				{
					_settings = value;
					if (_control != null)
					{
						// populate UI with new settings
						_control.Settings = _settings;
					}
				}
			}
		}

		private IssueRepositorySettings Reconcile(IssueRepositorySettings oldSettings, AnkhRepository newSettings)
		{
			//if (oldSettings == null)
			//{
			//	return newSettings;
			//}
			return newSettings;
		}

		private ConfigurationPage _control;

		private UserControl Control
		{
			get
			{
				if (_control == null)
				{
					ConfigurationPage control = new ConfigurationPage();
					control.OnPageEvent += new EventHandler<ConfigPageEventArgs>(control_OnPageEvent);
					_control = control;
				}
				return _control;
			}
		}

		void control_OnPageEvent(object sender, ConfigPageEventArgs e)
		{
			// raise page changed event to notify AnkhSVN
			base.ConfigurationPageChanged(e);
		}

		#region IWin32Window Members

		public IntPtr Handle
		{
			get { return Control.Handle; }
		}

		#endregion
	}
}
