using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

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
				foreach (var workItem in associatedWorkItems) result.AppendFormat("{0} {1}: {2}\n", workItem.type, workItem.id, workItem.title);
				return result.ToString();
			}
		}

		private WorkItemStore ConnectToTfs()
		{
			Uri tfsUri;
			if (!Uri.TryCreate(options.ServerName, UriKind.Absolute, out tfsUri))
				return null;

			var credentials = new System.Net.NetworkCredential();
			if (!string.IsNullOrEmpty(options.UserName))
			{
				credentials.UserName = options.UserName;
				credentials.Password = options.UserPassword;
			}

			_tfs = new TfsTeamProjectCollection(tfsUri, credentials);
			_tfs.Authenticate();

			return (WorkItemStore) _tfs.GetService(typeof (WorkItemStore));
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
				new Dictionary<string, string> {{"project", query.Query.Project.Name}},
				items, query.Query);

			return items;
		}

		private void AddQueryItem(ComboBox comboBox, QueryItem queryItem)
		{
			// Only add definitions to the combobox
			if (queryItem is QueryDefinition)
				comboBox.Items.Add(new TfsQuery(queryItem));

			if (!(queryItem is QueryFolder))
				return;

			foreach (var subQueryItem in queryItem as QueryFolder)
				AddQueryItem(comboBox, subQueryItem);
		}

		private void PopulateComboBoxWithSavedQueries(ComboBox comboBox)
		{
			Project project = _workItemStore.Projects[options.ProjectName];

			foreach (var queryItem in project.QueryHierarchy)
				AddQueryItem(comboBox, queryItem);

			if (comboBox.Items.Count > 0)
				comboBox.SelectedIndex = 0;
		}

		private void MyIssuesForm_Load(object sender, EventArgs e)
		{
			_workItemStore = ConnectToTfs();
			PopulateComboBoxWithSavedQueries(queryComboBox);
		}

		private void PopulateWorkItemsList(ListView listView, TfsQuery tfsQuery)
		{
			listView.Items.Clear();
			IEnumerable<MyWorkItem> workItems = GetWorkItems(tfsQuery);
			foreach (var workItem in workItems)
			{
				var lvi = new ListViewItem {Text = "", Tag = workItem,};
				lvi.SubItems.Add(workItem.type);
				lvi.SubItems.Add(workItem.id.ToString());
				lvi.SubItems.Add(workItem.state);
				lvi.SubItems.Add(workItem.title);
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
			PopulateWorkItemsList(listViewIssues, (TfsQuery) queryComboBox.SelectedItem);
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
	}

	internal class TfsQuery
	{
		public TfsQuery(QueryItem query)
		{
			Query = query;
		}

		public QueryItem Query { get; private set; }

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