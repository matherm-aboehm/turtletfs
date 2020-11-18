using System;
using System.ComponentModel;
using System.Windows.Forms;
using Ankh.ExtensionPoints.IssueTracker;
using Ankh.UI;
using TurtleTfs;
using TurtleTfs.Controls;

namespace Ankh.TfsProvider.Extension.IssueTracker
{
	/// <summary>
	/// Configuration page 
	/// </summary>
	class AnkhConfigurationPage : IssueRepositoryConfigurationPage, IWin32Window
	{
		readonly IAnkhPackage _package;
		IssueRepositorySettings _settings;
		private bool _processedSettings = false;
		private bool _canProcessSettings = false;

		public AnkhConfigurationPage(IAnkhPackage package)
		{
			_package = package;
		}

		/// <summary>
		/// Gets or Sets the config settings
		/// </summary>
		public override IssueRepositorySettings Settings
		{
			get
			{
				if (_control != null)
					return CreateRepositorySettings();
				if (_settings != null)
					return _settings;
				return null;
			}
			set
			{
				if (value != null &&
					string.Equals(value.ConnectorName, AnkhConnector.ConnectorName))
				{
					_settings = value;
					if (_control != null)
						SelectSettings();
				}
			}
		}

		private AnkhRepository CreateRepositorySettings()
		{
			return new AnkhRepository(_package, _control.Options);
		}

		/// <summary>
		/// populate UI with existing settings
		/// </summary>
		private void SelectSettings()
		{
			if (_settings != null
				&& !_processedSettings
				&& _canProcessSettings)
			{
				try
				{
					AnkhRepository repo = _settings as AnkhRepository;
					var options = repo?.Options ?? AnkhRepository.CreateTfsProviderOptions(_settings);
					_control.Options = options;
				}
				finally
				{
					_processedSettings = true;
				}
			}
		}

		private OptionsControl _control;

		private OptionsControl Control
		{
			get
			{
				if (_control == null)
				{
					_control = new OptionsControl();
					_control.Validating += Control_Validating;
					_control.Validated += Control_Validated;
					_control.Load += Control_Load;
				}
				return _control;
			}
		}

		private void Control_Load(object sender, EventArgs e)
		{
			_canProcessSettings = true;
			SelectSettings();
		}

		private void Control_Validating(object sender, CancelEventArgs e)
		{
			if (e.Cancel)
			{
				ConfigPageEventArgs args = new ConfigPageEventArgs();
				args.IsComplete = false;
				args.Exception = Control.Error;
				// raise page changed event to notify AnkhSVN
				base.ConfigurationPageChanged(args);
			}
		}

		private void Control_Validated(object sender, EventArgs e)
		{
			ConfigPageEventArgs args = new ConfigPageEventArgs();
			args.IsComplete = true;

			// raise page changed event to notify AnkhSVN
			base.ConfigurationPageChanged(args);
		}

		#region IWin32Window Members

		public IntPtr Handle
		{
			get { return Control.Handle; }
		}

		#endregion
	}
}
