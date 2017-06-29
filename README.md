# Azure RestApi helpers

Headers in Azure Rest Api is a pain! Since I believe there are a lot of you out there that hates configuration and just want to add things to the storage account, I open sourced this repository. 

This repository is inspired by https://github.com/cmfaustino/PROMPT11-10-Cloud-Computing.cmfaustino/tree/master/Storage_REST_CS/Storage_REST/StorageSampleREST. This repo really saved me a lot of headache.

The repository will be updated with more methods when I need them, please feel free to create pull requests with missing parts.

The repository is built as NetStandard and by that possible to use both in Asp.Net Core and older NET Framework.

~ forever 

## nuget

Project can be found as nuget package at https://www.nuget.org/packages/Ainteger.Azure.RestApi/

To install Azure Restapi helpers, run the following command in the Package Manager Console

	PM> Install-Package Ainteger.Azure.RestApi
	
To update Azure Restapi helpers, run the following command in the Package Manager Console
	
	PM> Update-Package Ainteger.Azure.RestApi

## Sample code

The code is really simple to use with IoC and this is an example of how to configurate

	public void ConfigureServices(IServiceCollection services)
	{
		services.AddSingleton(typeof(StorageAuthentication), new StorageAuthentication { AccountName = "StorageAccountName", StorageKey = "StorageKey" });
		services.AddSingleton<IAzureStorageHandler, AzureStorageHandler>();
		services.AddScoped<IWebRequest, WebRequest>();
		services.AddScoped<ITable, Table>();
		services.AddScoped<IQueue, Queue>();
		services.AddScoped<IBlob, Blob>();
	}
