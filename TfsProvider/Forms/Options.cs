using System;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using VSS = Microsoft.VisualStudio.Services.Common;

namespace TurtleTfs.Forms
{
	public partial class Options : Form
	{
		public Options(string parameters)
		{
			InitializeComponent();
			Parameters = parameters;
		}

		public string Parameters { get=> optionsControl.Parameters; private set => optionsControl.Parameters = value; }
	}
}
