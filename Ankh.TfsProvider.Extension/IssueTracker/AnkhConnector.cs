using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Ankh.ExtensionPoints.IssueTracker;
using Ankh.UI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;

namespace Ankh.TfsProvider.Extension.IssueTracker
{
	/// <summary>
	/// TFS Issue Repository connector for AnkhSVN
	/// </summary>
	[Guid("4862431D-068D-49E6-9212-7BD764CB1BD0")]
	public class AnkhConnector : IssueRepositoryConnector
	{
		internal const string ConnectorName = "TfsProvider IssueTracker Connector";
		//TODO: implement this with AsyncLazy?
		/*private readonly AsyncLazy<IAnkhPackage> _package;

		public AnkhConnector(TfsProviderPackage package)
		{
			_package = new AsyncLazy<IAnkhPackage>(() => GetAnkhPackageAsync(package), package.JoinableTaskFactory);
		}

		internal static async System.Threading.Tasks.Task<IAnkhPackage> GetAnkhPackageAsync(IAsyncServiceProvider provider)
		{
			return await provider.GetServiceAsync(typeof(IAnkhPackage)) as IAnkhPackage;
		}

		internal IAnkhPackage AnkhPackage { get => _package.GetValueAsync().Result; }*/

		private readonly IAnkhPackage _package;

		public AnkhConnector(IAnkhPackage package)
		{
			_package = package;
		}

		internal static async Task<AnkhConnector> CreateAnkhConnectorAsync(IAsyncServiceProvider provider)
		{
			IAnkhPackage package = await provider.GetServiceAsync(typeof(IAnkhPackage)) as IAnkhPackage;
			return new AnkhConnector(package);
		}

		/// <summary>
		/// Gets the configuration page used in Issue Repository Setup dialog
		/// </summary>
		public override IssueRepositoryConfigurationPage ConfigurationPage
		{
			get { return new AnkhConfigurationPage(_package); }
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
				return AnkhRepository.Create(settings, _package);
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
