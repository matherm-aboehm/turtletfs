using System;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace TurtleTfs.Controls
{
	public partial class OptionsControl : UserControl
	{
		private TfsProviderOptions savedOptions;

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TfsProviderOptions Options
		{
			get
			{
				TfsProviderOptions options = new TfsProviderOptions();
				options.ServerName = tfsAddressTextBox.Text;
				options.UserName = tfsUsernameTextBox.Text;
				options.UserPassword = tfsPasswordTextBox.Text;
				options.VisualStudioOnline = tfsVisualStudioOnline.Checked;
				options.ProjectName = projectComboBox.Text;
				return options;
			}
			set
			{
				savedOptions = value;
				if (value != null)
				{
					tfsAddressTextBox.Text = value.ServerName;
					tfsUsernameTextBox.Text = value.UserName;
					tfsPasswordTextBox.Text = value.UserPassword;
					tfsVisualStudioOnline.Checked = value.VisualStudioOnline;
					if (IsHandleCreated)
						PopulateProjectNameComboBox();
				}
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Parameters { get => TfsOptionsSerializer.Serialize(Options); set => Options = TfsOptionsSerializer.Deserialize(value); }

		public bool IsValid { get => projectComboBox.SelectedIndex != -1; }

		public Exception Error { get; private set; }

		public OptionsControl()
		{
			InitializeComponent();
		}

		private void Options_Load(object sender, EventArgs e)
		{
			if (!DesignMode)
				PopulateProjectNameComboBox();
		}

		protected override void OnValidating(CancelEventArgs e)
		{
			if (!IsValid)
				e.Cancel = true;

			base.OnValidating(e);
		}

		private void PopulateProjectNameComboBox()
		{
			Error = null;
			projectComboBox.Items.Clear();
			bool needsValidation = true;

			try
			{
				try
				{
					if (savedOptions == null)
						return;
					tfsConnector.Options = savedOptions;
					tfsConnector.ConnectToTfs();
					var workItemStore = tfsConnector.GetService<WorkItemStore>();
					if (workItemStore == null)
						return;

					foreach (Project project in workItemStore.Projects)
						projectComboBox.Items.Add(project.Name);
				}
				catch (Exception ex)
				{
					//MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					Error = ex;
					return;
				}

				int existingProjectIndex = projectComboBox.Items.Count > 0 ? 0 : -1;
				if (!string.IsNullOrEmpty(savedOptions?.ProjectName))
					existingProjectIndex = projectComboBox.Items.IndexOf(savedOptions.ProjectName);
				needsValidation = existingProjectIndex == -1;
				projectComboBox.SelectedIndex = existingProjectIndex;
			}
			finally
			{
				if (needsValidation)
					ValidateSelf();
			}
		}

		private void refreshProjectsButton_Click(object sender, EventArgs e)
		{
			savedOptions = Options;
			PopulateProjectNameComboBox();
		}

		public static ContainerControl GetParentContainerControl(Control child)
		{
			Control parent = child;
			while ((parent = parent.Parent) != null)
				if (parent is ContainerControl parentContainer)
					return parentContainer;
			return null;
		}

		private void ValidateSelf()
		{
			ContainerControl validationHelper = GetParentContainerControl(this);
			bool needDispose = false;
			if (validationHelper == null)
			{
				validationHelper = new ContainerControl();
				needDispose = true;
				validationHelper.Controls.Add(this);
			}
			try
			{
				validationHelper.ValidateChildren();
			}
			finally
			{
				if (needDispose)
				{
					validationHelper.Controls.Remove(this);
					validationHelper.Dispose();
				}
			}
		}

		private void projectComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			ValidateSelf();
		}
	}
}
