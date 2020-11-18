using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;

namespace Ankh.TfsProvider.Extension
{
	internal static class ServiceProviderExtensions
	{

		public static T GetService<T>(this IServiceProvider provider)
		{
			return (T)provider.GetService(typeof(T));
		}

		public static async Task<T> GetServiceAsync<T>(this IAsyncServiceProvider provider)
		{
			return (T)await provider.GetServiceAsync(typeof(T));
		}
	}
}
