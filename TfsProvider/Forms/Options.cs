using System;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using VSS = Microsoft.VisualStudio.Services.Common;

namespace TurtleTfs.Forms
{
	public partial class Options : Form
	{
		private readonly TfsProviderOptions options;

		public Options(string parameters)
		{
			InitializeComponent();
			Parameters = parameters;
			options = TfsOptionsSerializer.Deserialize(parameters);
			tfsAddressTextBox.Text = options.ServerName;
			tfsUsernameTextBox.Text = options.UserName;
			tfsPasswordTextBox.Text = options.UserPassword;
			tfsVisualStudioOnline.Checked = options.VisualStudioOnline;
		}

		public string Parameters { get; private set; }

		private void okButton_Click(object sender, EventArgs e)
		{
			options.ServerName = tfsAddressTextBox.Text;
			options.UserName = tfsUsernameTextBox.Text;
			options.UserPassword = tfsPasswordTextBox.Text;
			options.VisualStudioOnline = tfsVisualStudioOnline.Checked;
			options.ProjectName = projectComboBox.Text;
			Parameters = TfsOptionsSerializer.Serialize(options);
		}

		private void Options_Load(object sender, EventArgs e)
		{
			PopulateProjectNameComboBox();
		}

		private void PopulateProjectNameComboBox()
		{
			string url = tfsAddressTextBox.Text;
			string username = tfsUsernameTextBox.Text;
			string password = tfsPasswordTextBox.Text;

			Uri tfsUri;
			if (!Uri.TryCreate(url, UriKind.Absolute, out tfsUri))
				return;

			var credentials = new VSS.VssCredentials();
			if (!string.IsNullOrEmpty(username))
			{
				credentials = new VSS.VssCredentials(
					new VSS.WindowsCredential(new System.Net.NetworkCredential(
						username, password)),
					VSS.CredentialPromptType.PromptIfNeeded);
			}

			var tfs = new TfsTeamProjectCollection(tfsUri, credentials);
			tfs.Authenticate();

			var workItemStore = tfs.GetService<WorkItemStore>();

			projectComboBox.Items.Clear();
			foreach (Project project in workItemStore.Projects)
				projectComboBox.Items.Add(project.Name);

			int existingProjectIndex = -1;
			if (!string.IsNullOrEmpty(options.ProjectName))
				existingProjectIndex = projectComboBox.Items.IndexOf(options.ProjectName);
			projectComboBox.SelectedIndex = existingProjectIndex > 0 ? existingProjectIndex : 0;
		}

		private void refreshProjectsButton_Click(object sender, EventArgs e)
		{
			PopulateProjectNameComboBox();
		}
	}
}
