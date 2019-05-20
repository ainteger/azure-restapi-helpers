# Azure RestApi helpers

Headers in Azure Rest Api is a pain! Since I believe there are a lot of you out there that hates configuration and just want to add things to the storage account, I open sourced this repository. 

This repository is inspired by https://github.com/cmfaustino/PROMPT11-10-Cloud-Computing.cmfaustino/tree/master/Storage_REST_CS/Storage_REST/StorageSampleREST. This repo really saved me a lot of headache.

The repository will be updated with more methods when I need them, please feel free to create pull requests with missing parts.

The repository is built as NetStandard2.0

~ forever 

## nuget

Project can be found as nuget package at https://www.nuget.org/packages/Ainteger.Azure.RestApi/

To install Azure Restapi helpers, run the following command

	PM> dotnet add package Ainteger.Azure.RestApi 

## Sample code

The code is really simple to use with IoC and this is an example of how to configurate. 

### Startup.cs

	public void ConfigureServices(IServiceCollection services)
	{
		services.AddAzureRestApi(options => { options.StorageAccountName = ""; options.StorageKey = ""; });	
	}

This will inject the three helpers as HttpClients and will take care of HttpClients reuse, so no need for singleton to avoid socket expections.

### Controller

Inject IAzureBlobClient, IAzureQueueClient or IAzureTableClient in the current constructor and it will make the request for you.

## How to build
	PM> cd src
	PM> dotnet build -c Release 