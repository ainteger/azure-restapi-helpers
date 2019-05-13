using Azure.RestApi;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class AzureRestApiFactoryExtensions
	{
		public static IServiceCollection AddAzureRestApi(this IServiceCollection services, Action<AzureRestApiOptions> configure)
		{
			if (configure == null)
			{
				throw new ArgumentNullException(nameof(configure));
			}

			services.AddTransient<IAzureStorageHandler, AzureStorageHandler>();
			
			services.AddHttpClient<IAzureTableClient, AzureTableClient>();
			services.AddHttpClient<IAzureBlobClient, AzureBlobClient>();
			services.AddHttpClient<IAzureQueueClient, AzureQueueClient>();

			services.Configure(configure);

			return services;
		}
	}
}
