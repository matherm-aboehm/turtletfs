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
		public IssuesBrowser(string parameters, string comment)
		{
			InitializeComponent();
			Comment = comment;
			issuesBrowserControl.Parameters = parameters;
		}

		public string Comment { get { return commentBox.Text; } private set { commentBox.Text = value; } }

		public string AssociatedWorkItems { get; private set; }

		private void okButton_Click(object sender, EventArgs e)
		{
			AssociatedWorkItems = issuesBrowserControl.AssociatedWorkItemsMessage;
		}
	}
}
