using System;
using System.Runtime.InteropServices;
using Interop;
using Interop.BugTraqProvider;
using Microsoft.Win32;
using TurtleTfs.Forms;

namespace TurtleTfs
{
	[ComVisible(true), Guid("75971BA7-A422-4100-89E4-79E9FC56C699"), ClassInterface(ClassInterfaceType.None)]
	[Category(Constants.CATID_BugTraqProvider)]
	public class TfsProvider : IBugTraqProvider2
	{
		[ComRegisterFunction]
		private static void ComRegister(Type comType)
		{
			object[] caCategories = comType.GetCustomAttributes(typeof(CategoryAttribute), true);
			if (caCategories.Length != 0)
			{
				string parentKeyPath = $"CLSID\\{comType.GUID.ToString("B")}\\Implemented Categories";
				var catKey = Registry.ClassesRoot.CreateSubKey(parentKeyPath);
				if (catKey == null)
					return;

				using (catKey)
				{
					foreach (CategoryAttribute category in caCategories)
					{
						catKey.CreateSubKey(category.Category.ToString("B"))?.Close();
					}
				}
			}
		}

		[ComUnregisterFunction]
		private static void ComUnregister(Type comType)
		{
			object[] caCategories = comType.GetCustomAttributes(typeof(CategoryAttribute), true);
			if (caCategories.Length != 0)
			{
				foreach (CategoryAttribute category in caCategories)
				{
					Registry.ClassesRoot.DeleteSubKey($"CLSID\\{comType.GUID.ToString("B")}\\Implemented Categories\\{category.Category.ToString("B")}", false);
				}
			}
		}

		#region IBugTraqProvider2 Members

		public bool ValidateParameters(IntPtr hParentWnd, string parameters)
		{
			bool result = true;
			try
			{
				TfsOptionsSerializer.Deserialize(parameters);
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public string GetLinkText(IntPtr hParentWnd, string parameters)
		{
			return Properties.Resources.WorkItems;
		}

		public string GetCommitMessage(IntPtr hParentWnd, string parameters, string commonRoot, string[] pathList, string originalMessage)
		{
			return FormLauncher.LaunchIssueBrowserForm(parameters, originalMessage);
		}

		public string GetCommitMessage2(IntPtr hParentWnd, string parameters, string commonURL, string commonRoot, string[] pathList, string originalMessage,
										string bugID, out string bugIDOut, out string[] revPropNames, out string[] revPropValues)
		{
			bugIDOut = bugID;
			revPropNames = new string[] { };
			revPropValues = new string[] { };
			return FormLauncher.LaunchIssueBrowserForm(parameters, originalMessage);
		}

		public string CheckCommit(IntPtr hParentWnd, string parameters, string commonURL, string commonRoot, string[] pathList, string commitMessage)
		{
			return string.Empty;
		}

		public string OnCommitFinished(IntPtr hParentWnd, string commonRoot, string[] pathList, string logMessage, int revision)
		{
			//Show form like "Would you like to mention this commit in Work Items' histories?"
			return string.Empty;
		}

		public bool HasOptions()
		{
			return true;
		}

		public string ShowOptionsDialog(IntPtr hParentWnd, string parameters)
		{
			return FormLauncher.LaunchOptionsForm(parameters);
		}

		#endregion
	}
}
