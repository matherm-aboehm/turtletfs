using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using Microsoft.VisualStudio.Threading;

namespace Ankh.TfsProvider.Extension
{
	internal static class TaskExtensions
	{
#pragma warning disable VSTHRD200 // Use suffix "Async" für asynchronous methods
		public static Task HandleExceptionOnCurrentThread(this Task task, Func<Exception, bool> handler = null)
		{
			SynchronizationContext current = SynchronizationContext.Current;
			return task.ContinueWith((parentTask, state) =>
			{
				var (context, handler2) = ((SynchronizationContext, Func<Exception, bool>))state;
#pragma warning disable VSTHRD001 // Post() doesn't wait, so this is not a synchronous API and can't deadlock.
				context.Post(state2 =>
#pragma warning restore VSTHRD001
				{
					var (exInfo, exHandler) = ((ExceptionDispatchInfo, Func<Exception, bool>))state2;
					if (exHandler == null || !exHandler(exInfo.SourceException))
						exInfo.Throw();
				}, (ExceptionDispatchInfo.Capture(parentTask.Exception), handler2));
			}, state: (current, handler), cancellationToken: default,
			continuationOptions: TaskContinuationOptions.OnlyOnFaulted, scheduler: TaskScheduler.Default);
		}

		public static Task HandleExceptionOnCurrentThread(this JoinableTask task, Func<Exception, bool> handler = null)
		{
			async Task catchException(JoinableTask task2, Func<Exception, bool> handler2, SynchronizationContext context)
			{
				try
				{
					await task2;
				}
				catch (Exception ex)
				{
#pragma warning disable VSTHRD001 // Post() does not wait, so this is not a synchronous API and can't deadlock.
					context.Post(state2 =>
#pragma warning restore VSTHRD001
					{
						var (exInfo, exHandler) = ((ExceptionDispatchInfo, Func<Exception, bool>))state2;
						if (exHandler == null || !exHandler(exInfo.SourceException))
							exInfo.Throw();
					}, (ExceptionDispatchInfo.Capture(ex), handler2));
				}
			}
			return catchException(task, handler, SynchronizationContext.Current);
		}

		static readonly PropertyInfo _piUnderlyingSynchronizationContext = typeof(JoinableTaskFactory).GetProperty("UnderlyingSynchronizationContext", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);


		//https://stackoverflow.com/questions/23431595/task-yield-real-usages
		public static async Task IdleYield()
#pragma warning restore VSTHRD200 // Use suffix "Async" für asynchronous methods
		{
			SynchronizationContext current = SynchronizationContext.Current;
			if (current != null && current.GetType() != typeof(SynchronizationContext))
			{
				if (current.GetType().FullName == "Microsoft.VisualStudio.Threading.JoinableTask+JoinableTaskSynchronizationContext")
				{
					current = (SynchronizationContext)_piUnderlyingSynchronizationContext.GetValue(TfsProviderPackage.Instance.JoinableTaskFactory);
				}
				if (current is DispatcherSynchronizationContext)
				{
					await Dispatcher.Yield(DispatcherPriority.ApplicationIdle);
					return;
				}
				if (current is WindowsFormsSynchronizationContext)
				{
					var idleTcs = new TaskCompletionSource<object>();
					// subscribe to Application.Idle
					EventHandler handler = null;
					handler = (s, e) =>
					{
						System.Windows.Forms.Application.Idle -= handler;
						idleTcs.SetResult(null);
					};
					System.Windows.Forms.Application.Idle += handler;
					await idleTcs.Task;
					return;
				}
			}

			await Task.Yield();
			return;
		}

		public static IDisposable RegisterHangDetection(this JoinableTaskContextNode node, Action<JoinableTaskContext.HangDetails> hangDetectionHandler)
		{
			if (node is AsyncPackageJoinableTaskContextNode asyncNode)
				return asyncNode.RegisterHangDetection(hangDetectionHandler);
			throw new NotSupportedException();
		}
	}
}
