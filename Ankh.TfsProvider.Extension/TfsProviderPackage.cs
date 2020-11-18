﻿using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using Task = System.Threading.Tasks.Task;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using Microsoft.VisualStudio.Threading;

namespace Ankh.TfsProvider.Extension
{
	/// <summary>
	/// This is the class that implements the package exposed by this assembly.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The minimum requirement for a class to be considered a valid package for Visual Studio
	/// is to implement the IVsPackage interface and register itself with the shell.
	/// This package uses the helper classes defined inside the Managed Package Framework (MPF)
	/// to do it: it derives from the Package class that provides the implementation of the
	/// IVsPackage interface and uses the registration attributes defined in the framework to
	/// register itself and its components with the shell. These attributes tell the pkgdef creation
	/// utility what data to put into .pkgdef file.
	/// </para>
	/// <para>
	/// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
	/// </para>
	/// </remarks>
	[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
	[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About

	// In order be loaded inside Visual Studio in a machine that has not the VS SDK installed, 
	// package needs to have a valid load key (it can be requested at 
	// http://msdn.microsoft.com/vstudio/extend/). This attributes tells the shell that this 
	// package has a load key embedded in its resources.
	//[ProvideLoadKey("Standard", "1.0", AppConstants.PackageName, AppConstants.CompanyName, 1)]
	[Guid(TfsProviderPackage.PackageGuidString)]
	[ProvideService(typeof(IssueTracker.AnkhConnector), ServiceName = IssueTracker.AnkhConnector.ConnectorName, IsAsyncQueryable = true)]
	[ProvideIssueRepositoryConnector(typeof(IssueTracker.AnkhConnector), IssueTracker.AnkhConnector.ConnectorName, typeof(TfsProviderPackage), "#110")]
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
	public sealed class TfsProviderPackage : AsyncPackage
	{
		/// <summary>
		/// TfsProviderPackage GUID string.
		/// </summary>
		public const string PackageGuidString = "2f3b3d10-e8e7-4078-b59e-ac87c6143135";

		static Guid IID_IUnknown = VSConstants.IID_IUnknown;

		public static TfsProviderPackage Instance { get; private set; }

		AsyncPackageJoinableTaskContextNode m_taskContextNode;
		//HINT: This doesn't need to be thread-safe, we only need a simple and single point of object creation and storage,
		//not a pure singleton. Race conditions are allowed and wouldn't change the behavior.
		public JoinableTaskContextNode JoinableTaskContextNode => m_taskContextNode ??
			(m_taskContextNode = new AsyncPackageJoinableTaskContextNode(this));

		/// <summary>
		/// Initializes a new instance of the <see cref="TfsProviderPackage"/> class.
		/// </summary>
		public TfsProviderPackage()
		{
			Instance = this;
			// Inside this method you can place any initialization code that does not require
			// any Visual Studio service because at this point the package object is created but
			// not sited yet inside Visual Studio environment. The place to do all the other
			// initialization is the Initialize method.
			Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
		}

		#region Package Members

		/// <summary>
		/// Initialization of the package; this method is called right after the package is sited, so this is the place
		/// where you can put all the initialization code that rely on services provided by VisualStudio.
		/// </summary>
		/// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
		/// <param name="progress">A provider for progress updates.</param>
		/// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
		protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
		{
			Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
			await base.InitializeAsync(cancellationToken, progress);

			IAsyncServiceContainer ctrnr = this;
			ctrnr.AddService(typeof(IssueTracker.AnkhConnector), new AsyncServiceCreatorCallback(
				async (c, ct, type) =>
				{
					if (c == this && type == typeof(IssueTracker.AnkhConnector))
						return await IssueTracker.AnkhConnector.CreateAnkhConnectorAsync(this);
					return null;
				}), true);

			// When initialized asynchronously, the current thread may be a background thread at this point.
			// Do any initialization that requires the UI thread after switching to the UI thread.
			await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
		}

		#endregion

		public T QueryService<T>(Guid serviceGuid) where T : class
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			var sp = this.GetService<IOleServiceProvider>();

			if (sp == null)
				return null;

			if (!VSErr.Succeeded(sp.QueryService(ref serviceGuid, ref IID_IUnknown, out var handle))
				|| handle == IntPtr.Zero)
				return null;

			try
			{
				object obj = Marshal.GetObjectForIUnknown(handle);

				return obj as T;
			}
			finally
			{
				Marshal.Release(handle);
			}
		}

		public async Task<T> QueryServiceAsync<T>(Guid serviceGuid) where T : class
		{
			var sp = await this.GetServiceAsync<IOleServiceProvider>();

			if (sp == null)
				return null;

			await this.JoinableTaskFactory.SwitchToMainThreadAsync();

			if (!VSErr.Succeeded(sp.QueryService(ref serviceGuid, ref IID_IUnknown, out var handle))
				|| handle == IntPtr.Zero)
				return null;

			try
			{
				object obj = Marshal.GetObjectForIUnknown(handle);

				return obj as T;
			}
			finally
			{
				Marshal.Release(handle);
			}
		}
	}
}
