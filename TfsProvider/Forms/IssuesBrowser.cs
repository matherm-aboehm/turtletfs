using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.ProcessConfiguration.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using VSS = Microsoft.VisualStudio.Services.Common;

namespace TurtleTfs.Forms
{
	public partial class IssuesBrowser : Form
	{
		private readonly List<MyWorkItem> associatedWorkItems = new List<MyWorkItem>();
		private readonly ListViewColumnSorter listViewColumnSorter;
		private readonly TfsProviderOptions options;
		private TfsTeamProjectCollection _tfs;
		private WorkItemStore _workItemStore;

		public IssuesBrowser(string parameters, string comment)
		{
			InitializeComponent();
			Comment = comment;
			options = TfsOptionsSerializer.Deserialize(parameters);

			ColumnHeader idColumnHeader = listViewIssues.Columns.Cast<ColumnHeader>().FirstOrDefault(header => header.Name == "ID");
			int idColumnIndex = idColumnHeader == null ? 2 : idColumnHeader.Index;
			listViewColumnSorter = new ListViewColumnSorter(idColumnIndex);
			listViewIssues.ListViewItemSorter = listViewColumnSorter;
		}

		public string Comment { get { return commentBox.Text; } private set { commentBox.Text = value; } }

		public string AssociatedWorkItems
		{
			get
			{
				var result = new StringBuilder();
				result.AppendFormat(Properties.Resources.AssociatedWorkItems, string.Join("; ", associatedWorkItems.Select(w => "#" + w.id.ToString())));
				//foreach (var workItem in associatedWorkItems) result.AppendFormat("{0} {1}: {2}{3}", workItem.type, workItem.id, workItem.title, Environment.NewLine);
				return result.ToString();
			}
		}

		private WorkItemStore ConnectToTfs()
		{
			Uri tfsUri;
			if (!Uri.TryCreate(options.ServerName, UriKind.Absolute, out tfsUri))
				return null;

			var credentials = new VSS.VssCredentials();
			if (!string.IsNullOrEmpty(options.UserName))
			{
				credentials = new VSS.VssCredentials(
					new VSS.WindowsCredential(new System.Net.NetworkCredential(
						options.UserName, options.UserPassword)),
					VSS.CredentialPromptType.PromptIfNeeded);
			}

			_tfs = new TfsTeamProjectCollection(tfsUri, credentials);
			_tfs.Authenticate();

			return _tfs.GetService<WorkItemStore>();
		}

		private void RecurseWorkItems(Dictionary<string, string> context, List<MyWorkItem> items, QueryItem queryItem)
		{
			// Skip queries that aren't of type list, there's some funny exception i dont want to deal with
			var queryDefinition = queryItem as QueryDefinition;
			if (queryDefinition == null || queryDefinition.QueryType != QueryType.List)
				return;

			foreach (WorkItem workItem in _workItemStore.Query(queryDefinition.QueryText, context))
			{
				items.Add(new MyWorkItem
				{
					id = workItem.Id,
					state = workItem.State,
					title = workItem.Title,
					type = workItem.Type.Name
				});
			}
		}

		private IEnumerable<MyWorkItem> GetWorkItems(TfsQuery query)
		{
			var items = new List<MyWorkItem>();

			RecurseWorkItems(
				new Dictionary<string, string> {
					{ "project", query.Query.Project.Name },
					{ "currentIteration", query.CurrentIteration }
				}, items, query.Query);

			return items;
		}

		private void AddQueryItem(ComboBox comboBox, QueryItem queryItem, string currentIteration)
		{
			// Only add definitions to the combobox
			if (queryItem is QueryDefinition)
				comboBox.Items.Add(new TfsQuery(queryItem, currentIteration));

			if (!(queryItem is QueryFolder))
				return;

			foreach (var subQueryItem in queryItem as QueryFolder)
				AddQueryItem(comboBox, subQueryItem, currentIteration);
		}

		private string GetCurrentIterationPath(Project project)
		{
			var teamSettingsStore = _tfs.GetService<TeamSettingsConfigurationService>();
			var teamService = _tfs.GetService<TfsTeamService>();
			var currentUser = _tfs.AuthorizedIdentity;
			string projectUri = project.Uri.ToString();
			var team = teamService.QueryTeams(currentUser.Descriptor)
				.Where(t => t.Project == projectUri)
				.FirstOrDefault(); //TODO: if there is more than one team, provide selection mechanism

			if (team == null)
				throw new InvalidOperationException(
					string.Format(Properties.Resources.NotMemberOfTeamProject, currentUser.UniqueName, project.Name));

			var settings = teamSettingsStore.GetTeamConfigurationsForUser(new[] { projectUri })
				.Where(c => c.TeamName == team.Name)
				.FirstOrDefault();

			if (settings == null)
				throw new InvalidOperationException(
					string.Format(Properties.Resources.NoAccessToTeamProject, currentUser.UniqueName, project.Name));

			return settings.TeamSettings.CurrentIterationPath;
		}

		private void PopulateComboBoxWithSavedQueries(ComboBox comboBox)
		{
			Project project = _workItemStore.Projects[options.ProjectName];
			string currentIteration = GetCurrentIterationPath(project);

			foreach (var queryItem in project.QueryHierarchy)
				AddQueryItem(comboBox, queryItem, currentIteration);

			if (comboBox.Items.Count > 0)
				comboBox.SelectedIndex = 0;
		}

		private void MyIssuesForm_Load(object sender, EventArgs e)
		{
			_workItemStore = ConnectToTfs();
			PopulateComboBoxWithSavedQueries(queryComboBox);
		}

		private string FormatWorkItemAddress(int workId)
		{
			// https://example.visualstudio.com/DefaultCollection/Example/_workitems#_a=edit&id=123
			return string.Format("{0}/{1}/_workitems#_a=edit&id={2}", options.ServerName, options.ProjectName, workId);
		}

		private void PopulateWorkItemsList(ListView listView, TfsQuery tfsQuery)
		{
			listView.Items.Clear();
			IEnumerable<MyWorkItem> myWorkItems = GetWorkItems(tfsQuery);
			foreach (var myWorkItem in myWorkItems)
			{
				var lvi = new ListViewItem
				{
					Text = "",
					Tag = myWorkItem,
					UseItemStyleForSubItems = !options.VisualStudioOnline
				};

				lvi.SubItems.Add(myWorkItem.type);
				if (!options.VisualStudioOnline)
				{
					lvi.SubItems.Add(myWorkItem.id.ToString());
				}
				else
				{
					lvi.SubItems.Add(new ListViewItem.ListViewSubItem
					{
						Text = myWorkItem.id.ToString(),
						Font = new Font(lvi.Font, FontStyle.Underline),
						ForeColor = Color.Blue
					});
				}
				lvi.SubItems.Add(myWorkItem.state);
				lvi.SubItems.Add(myWorkItem.title);
				listView.Items.Add(lvi);
			}
			foreach (ColumnHeader column in listViewIssues.Columns) column.Width = -1;
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem lvi in listViewIssues.Items.Cast<ListViewItem>().Where(lvi => lvi.Checked))
				associatedWorkItems.Add((MyWorkItem)lvi.Tag);
		}

		private void queryComboBox_SelectedValueChanged(object sender, EventArgs e)
		{
			PopulateWorkItemsList(listViewIssues, (TfsQuery)queryComboBox.SelectedItem);
		}

		private void listViewIssues_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			if (e.Column == listViewColumnSorter.SortColumn)
				listViewColumnSorter.InvertOrder();
			else
			{
				listViewColumnSorter.SortColumn = e.Column;
				listViewColumnSorter.Order = SortOrder.Ascending;
			}
			listViewIssues.Sort();
		}

		private void listViewIssues_Click(object sender, EventArgs e)
		{
			if (!options.VisualStudioOnline)
				return;

			Point mousePosition = listViewIssues.PointToClient(Control.MousePosition);
			ListViewHitTestInfo hit = listViewIssues.HitTest(mousePosition);
			if (hit.SubItem == null)
				return;

			int columnindex = hit.Item.SubItems.IndexOf(hit.SubItem);
			if (columnindex != 2)
				return;

			var workItem = (MyWorkItem)hit.Item.Tag;
			System.Diagnostics.Process.Start(FormatWorkItemAddress(workItem.id));
		}
	}

	internal class TfsQuery
	{
		public TfsQuery(QueryItem query, string currentIteration)
		{
			Query = query;
			CurrentIteration = currentIteration;
		}

		public QueryItem Query { get; private set; }
		public string CurrentIteration { get; private set; }

		public override string ToString()
		{
			if (Query == null)
				return string.Empty;

			// Skip the project name
			return Query.Path.Substring(Query.Project.Name.Length + 1);
		}
	}

	public struct MyWorkItem
	{
		public MyWorkItem(int id, string state, string title, string type)
		{
			this.id = id;
			this.state = state;
			this.title = title;
			this.type = type;
		}

		public int id;
		public string state;
		public string title;
		public string type;
	}
}
