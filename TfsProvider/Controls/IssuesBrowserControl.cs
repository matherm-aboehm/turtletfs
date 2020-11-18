using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.ProcessConfiguration.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace TurtleTfs.Controls
{
	public partial class IssuesBrowserControl : UserControl
	{
		private static readonly object EventOpenIssue = new object();
		private readonly ListViewColumnSorter listViewColumnSorter;
		private WorkItemStore _workItemStore;

		public IssuesBrowserControl()
		{
			InitializeComponent();

			listViewColumnSorter = new ListViewColumnSorter(idColumn.Index);
			listViewIssues.ListViewItemSorter = listViewColumnSorter;
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string AssociatedWorkItemsMessage
		{
			get
			{
				var result = new StringBuilder();
				if (AssociatedWorkItems.Any())
					result.AppendFormat(Properties.Resources.AssociatedWorkItems, string.Join("; ", AssociatedWorkItems.Select(w => "#" + w.id.ToString())));
				//foreach (var workItem in AssociatedWorkItems) result.AppendFormat("{0} {1}: {2}{3}", workItem.type, workItem.id, workItem.title, Environment.NewLine);
				return result.ToString();
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IEnumerable<int> AssociatedWorkItemIds => AssociatedWorkItems.Select(w => w.id);

		private IEnumerable<MyWorkItem> AssociatedWorkItems => from ListViewItem lvi in listViewIssues.Items
															   where lvi.Checked
															   select ((MyWorkItem)lvi.Tag);

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TfsProviderOptions Options { get; set; } = new TfsProviderOptions();

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Parameters { get => TfsOptionsSerializer.Serialize(Options); set => Options = TfsOptionsSerializer.Deserialize(value); }

		public event EventHandler<OpenIssueEventArgs> OpenIssue
		{
			add { Events.AddHandler(EventOpenIssue, value); }
			remove { Events.RemoveHandler(EventOpenIssue, value); }
		}

		protected virtual void OnOpenIssue(OpenIssueEventArgs e)
		{
			var eventHandler = (EventHandler<OpenIssueEventArgs>)Events[EventOpenIssue];
			if (eventHandler != null)
				eventHandler(this, e);
			else
				System.Diagnostics.Process.Start(FormatWorkItemAddress(e.IssueId, Options));
		}

		public IDisposable AcquireConnectionContext()
		{
			return tfsConnector.AcquireConnectionContext();
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

		private static void AddQueryItem(ComboBox comboBox, QueryItem queryItem, string currentIteration)
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
			var teamSettingsStore = tfsConnector.GetService<TeamSettingsConfigurationService>();
			var teamService = tfsConnector.GetService<TfsTeamService>();
			var currentUser = tfsConnector.AuthorizedIdentity;
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

		private void PopulateComboBoxWithSavedQueries()
		{
			tfsConnector.Options = Options;
			tfsConnector.ConnectToTfs();

			_workItemStore = tfsConnector.GetService<WorkItemStore>();
			if (_workItemStore != null)
			{
				Project project = _workItemStore.Projects[Options.ProjectName];
				string currentIteration = GetCurrentIterationPath(project);

				foreach (var queryItem in project.QueryHierarchy)
					AddQueryItem(queryComboBox, queryItem, currentIteration);

				if (queryComboBox.Items.Count > 0)
					queryComboBox.SelectedIndex = 0;
			}
		}

		private void IssuesBrowserControl_Load(object sender, EventArgs e)
		{
			if (!DesignMode)
				PopulateComboBoxWithSavedQueries();
		}

		public static string FormatWorkItemAddress(int workId, TfsProviderOptions options)
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
					UseItemStyleForSubItems = !Options.VisualStudioOnline
				};

				lvi.SubItems.Add(myWorkItem.type);
				if (!Options.VisualStudioOnline)
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
			if (!Options.VisualStudioOnline)
				return;

			Point mousePosition = listViewIssues.PointToClient(Control.MousePosition);
			ListViewHitTestInfo hit = listViewIssues.HitTest(mousePosition);
			if (hit.SubItem == null)
				return;

			int columnindex = hit.Item.SubItems.IndexOf(hit.SubItem);
			if (columnindex != idColumn.Index)
				return;

			var workItem = (MyWorkItem)hit.Item.Tag;
			OnOpenIssue(new OpenIssueEventArgs(workItem.id));
		}
	}

	public class OpenIssueEventArgs : EventArgs
	{
		public int IssueId { get; }

		internal OpenIssueEventArgs(int issueId)
		{
			IssueId = issueId;
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

	internal struct MyWorkItem
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
