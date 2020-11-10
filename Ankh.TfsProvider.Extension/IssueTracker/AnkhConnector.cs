using System.Runtime.InteropServices;
using Ankh.ExtensionPoints.IssueTracker;

namespace Ankh.TfsProvider.Extension.IssueTracker
{
	/// <summary>
	/// TFS Issue Repository connector for AnkhSVN
	/// </summary>
	[Guid("4862431D-068D-49E6-9212-7BD764CB1BD0")]
	public class AnkhConnector : IssueRepositoryConnector
	{
		internal const string ConnectorName = "TfsProvider IssueTracker Connector";

		public AnkhConnector()
		{ }

		/// <summary>
		/// Gets the configuration page used in Issue Repository Setup dialog
		/// </summary>
		public override IssueRepositoryConfigurationPage ConfigurationPage
		{
			get { return new AnkhConfigurationPage(); }
		}

		/// <summary>
		/// Create an Issue repository based on the given settings
		/// </summary>
		/// <param name="settings"></param>
		/// <returns></returns>
		public override IssueRepository Create(IssueRepositorySettings settings)
		{
			if (settings != null && string.Equals(settings.ConnectorName, Name))
			{
				return AnkhRepository.Create(settings);
			}
			return null;
		}

		/// <summary>
		/// Gets the connector name used in the Issue connector drop-down
		/// </summary>
		/// <remarks>Needs to be unique</remarks>
		public override string Name
		{
			get { return ConnectorName; }
		}
	}
}
