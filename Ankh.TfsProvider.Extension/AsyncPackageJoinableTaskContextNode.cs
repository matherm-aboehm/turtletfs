using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;

namespace Ankh.TfsProvider.Extension
{
	public class AsyncPackageJoinableTaskContextNode : JoinableTaskContextNode
	{
		private class DelegateHangDetectionRegistration : IDisposable
		{
			AsyncPackageJoinableTaskContextNode ownerNode;
			public DelegateHangDetectionRegistration(AsyncPackageJoinableTaskContextNode ownerNode, Action<JoinableTaskContext.HangDetails> hangDetectionHandler)
			{
				this.ownerNode = ownerNode;
				ownerNode.m_registrations.TryAdd(this, hangDetectionHandler);
			}
			public void Dispose()
			{
				ownerNode.m_registrations.TryRemove(this, out var _);
				if (ownerNode.m_registrations.Count == 0)
				{
					if (Interlocked.CompareExchange(ref ownerNode.m_syncRegistration, 0, 1) == 1)
					{
						var globalRegistration = Interlocked.Exchange(ref ownerNode.m_globalRegistration, null);
						globalRegistration?.Dispose();
					}
				}
				ownerNode = null;
			}
		}

		JoinableTaskFactory m_factory;
		ConcurrentDictionary<DelegateHangDetectionRegistration, Action<JoinableTaskContext.HangDetails>> m_registrations =
			new ConcurrentDictionary<DelegateHangDetectionRegistration, Action<JoinableTaskContext.HangDetails>>();
		volatile int m_syncRegistration;
		IDisposable m_globalRegistration;

		public AsyncPackageJoinableTaskContextNode(AsyncPackage package) : base(package.JoinableTaskFactory.Context)
		{
			m_factory = package.JoinableTaskFactory;
		}

		protected override JoinableTaskFactory CreateDefaultFactory()
		{
			return m_factory;
		}

		protected override void OnHangDetected(JoinableTaskContext.HangDetails details)
		{
			base.OnHangDetected(details);

			foreach (var item in m_registrations)
				item.Value(details);
		}

		public IDisposable RegisterHangDetection(Action<JoinableTaskContext.HangDetails> hangDetectionHandler)
		{
			if (m_globalRegistration == null)
			{
				if (Interlocked.CompareExchange(ref m_syncRegistration, 1, 0) == 0)
					Interlocked.Exchange(ref m_globalRegistration, RegisterOnHangDetected());
			}
			return new DelegateHangDetectionRegistration(this, hangDetectionHandler);
		}
	}
}
