using Azure.RestApi;
using System;


namespace Microsoft.Extensions.DependencyInjection
{
	public static class AzureRestApiFactoryExtensions
	{
		public static IServiceCollection AddAzureRestApi(this IServiceCollection services,  Action<AzureRestApiOptions> configure)
		{
			if (configure == null)
			{
				throw new ArgumentNullException(nameof(configure));
			}

			services.AddSingleton<IAzureStorageHandler, AzureStorageHandler>();
			services.AddSingleton<IBlobHandler, BlobHandler>();
			services.AddSingleton<IQueueHandler, QueueHandler>();
			services.AddSingleton<ITableHandler, TableHandler>();
			services.AddSingleton<IWebRequest, WebRequest>();

			services.Configure(configure);

			return services;
		}
	}
}
