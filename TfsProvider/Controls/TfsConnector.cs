using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using VSS = Microsoft.VisualStudio.Services.Common;

namespace TurtleTfs.Controls
{
	public partial class TfsConnector : Component, IServiceProvider
	{
		private class TfsConnectionWrapper : Component, ITfsConnectionDisposing
		{
			private static readonly object EventDisposing = new object();
			internal readonly TfsTeamProjectCollection _tfs;

			[Browsable(false)]
			[EditorBrowsable(EditorBrowsableState.Advanced)]
			public event EventHandler Disposing
			{
				add { Events.AddHandler(EventDisposing, value); }
				remove { Events.RemoveHandler(EventDisposing, value); }
			}

			public TfsConnectionWrapper(TfsTeamProjectCollection tfs)
			{
				_tfs = tfs;
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					lock (this)
					{
						EventHandler ed = (EventHandler)Events[EventDisposing];
						ed?.Invoke(this, EventArgs.Empty);

						if (_tfs != null)
							_tfs.Dispose();

						base.Dispose(true);

						Events.Dispose();
					}
				}
				else
					base.Dispose(false);
			}
		}

		private class ConnectionContext : IDisposable, IServiceProvider
		{
			private TfsConnectionWrapper _tfsConnection;

			public ConnectionContext(TfsConnectionWrapper tfsConnection)
			{
				_tfsConnection = tfsConnection;
				Monitor.Enter(tfsConnection);
			}

			public void Dispose()
			{
				Monitor.Exit(_tfsConnection);
				_tfsConnection = null;
			}

			object IServiceProvider.GetService(Type serviceType)
			{
				if (serviceType == typeof(TfsTeamProjectCollection))
					return _tfsConnection._tfs;
				if (serviceType == typeof(ITfsConnectionDisposing))
					return _tfsConnection;
				return null;
			}
		}

		private TfsConnectionWrapper _tfsConnection;

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TfsProviderOptions Options { get; set; } = new TfsProviderOptions();

		public TeamFoundationIdentity AuthorizedIdentity => _tfsConnection?._tfs.AuthorizedIdentity;

		public bool IsConnected => _tfsConnection != null;

		public TfsConnector()
		{
			InitializeComponent();
		}

		public TfsConnector(IContainer container)
		{
			container.Add(this);

			InitializeComponent();
		}

		public void ConnectToTfs()
		{
			Disconnect();
			Uri tfsUri;
			if (!Uri.TryCreate(Options.ServerName, UriKind.Absolute, out tfsUri))
				return;

			var credentials = new VSS.VssCredentials();
			if (!string.IsNullOrEmpty(Options.UserName))
			{
				credentials = new VSS.VssCredentials(
					new VSS.WindowsCredential(new System.Net.NetworkCredential(
						Options.UserName, Options.UserPassword)),
					VSS.CredentialPromptType.PromptIfNeeded);
			}

			_tfsConnection = new TfsConnectionWrapper(new TfsTeamProjectCollection(tfsUri, credentials));
			components.Add(_tfsConnection);
			_tfsConnection._tfs.Authenticate();
		}

		public void Disconnect()
		{
			var tfsConnection = _tfsConnection;
			_tfsConnection = null;
			if (tfsConnection != null)
			{
				components.Remove(tfsConnection);
				tfsConnection.Dispose();
			}
		}

		public IDisposable AcquireConnectionContext()
		{
			return new ConnectionContext(_tfsConnection);
		}

		public T GetService<T>()
		{
			if (typeof(T) == typeof(ISite) || typeof(T) == typeof(IContainer))
				return (T)base.GetService(typeof(T));
			if (_tfsConnection == null)
				return default(T);
			return _tfsConnection._tfs.GetService<T>();
		}

		protected override object GetService(Type service)
		{
			if (service == typeof(ISite) || service == typeof(IContainer))
				return base.GetService(service);
			return _tfsConnection?._tfs.GetService(service);
		}

		object IServiceProvider.GetService(Type serviceType)
		{
			return GetService(serviceType);
		}
	}

	public interface ITfsConnectionDisposing
	{
		event EventHandler Disposing;
	}
}
