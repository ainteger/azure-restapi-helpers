using Azure.RestApi.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Azure.RestApi
{
	public class AzureStorageHandler : IAzureStorageHandler
	{
		private string StorageAccount { get; }
		private string StorageKey { get; }

		public AzureStorageHandler(IOptions<AzureRestApiOptions> storageAuthentication)
		{
			StorageAccount = storageAuthentication.Value.StorageAccountName;
			StorageKey = storageAuthentication.Value.StorageKey;
		}

		public HttpRequestMessage GetRequest(StorageType storageType, HttpMethod method, string resource, byte[] requestBody = null, string ifMatch = "")
		{
			var now = DateTime.UtcNow;

			var request = new HttpRequestMessage(method, GetBaseUri(storageType) + resource);

			request.Headers.Add("x-ms-date", now.ToString("R", System.Globalization.CultureInfo.InvariantCulture));

			request.Headers.Add("x-ms-version", "2018-03-28");

			if (storageType == StorageType.Table)
			{
				request.Headers.Add("DataServiceVersion", "3.0;NetFx");
				request.Headers.Add("MaxDataServiceVersion", "3.0;NetFx");
				request.Headers.Add("Accept", "application/json;odata=nometadata");
			}
			else if (storageType == StorageType.Blob)
			{
				request.Headers.Add("x-ms-blob-type", "BlockBlob");
			}

			if (requestBody != null)
			{
				request.Content = new ByteArrayContent(requestBody);
				request.Content.Headers.Add("Content-Length", requestBody.Length.ToString());

				if (storageType == StorageType.Table)
				{
					request.Content.Headers.Add("Content-Type", "application/json;odata=nometadata");
				}

				if (storageType != StorageType.Blob)
				{
					request.Headers.Add("Accept-Charset", "UTF-8");
				}
			}

			if (!string.IsNullOrWhiteSpace(ifMatch))
			{
				request.Headers.Add("If-Match", ifMatch);
			}

			request.Headers.Add("Authorization", GetAuthorizationHeader(storageType, method, now, request, ifMatch));

			return request;
		}

		public string GetAuthorizationHeader(StorageType storageType, HttpMethod method, DateTime now, HttpRequestMessage request, string ifMatch)
		{
			if (storageType == StorageType.Table)
			{
				var messageSignature = string.Format("{0}\n{1}",
					now.ToString("R", System.Globalization.CultureInfo.InvariantCulture),
					$"/{StorageAccount}/{request.RequestUri.AbsolutePath.TrimStart('/')}");

				return $"SharedKeyLite {StorageAccount}:{GetSignature(messageSignature)}";
			}
			else
			{
				var messageSignature = string.Format("{0}\n\n\n{1}\n\n\n\n\n{2}\n\n\n\n{3}{4}",
					method,
					(method == HttpMethod.Get || method == HttpMethod.Head) ? string.Empty : request.Content?.Headers?.FirstOrDefault(x => x.Key == "Content-Length").Value.FirstOrDefault() ?? string.Empty,
					ifMatch,
					GetCanonicalizedHeaders(request),
					GetCanonicalizedResource(storageType, request.RequestUri, StorageAccount)
					);

				return $"SharedKey {StorageAccount}:{GetSignature(messageSignature)}";
			}
		}

		public string GetSignature(string messageSignature)
		{
			var signatureBytes = Encoding.UTF8.GetBytes(messageSignature);
			var SHA256 = new System.Security.Cryptography.HMACSHA256(Convert.FromBase64String(StorageKey));
			return Convert.ToBase64String(SHA256.ComputeHash(signatureBytes));
		}

		public string GetCanonicalizedHeaders(HttpRequestMessage request)
		{
			var sortedHeaders = request.Headers.Where(x => x.Key.ToLowerInvariant().StartsWith("x-ms-", StringComparison.Ordinal))
				.OrderBy(x => x.Key);

			return string.Join(string.Empty,
				sortedHeaders.Select(x =>
					$"{x.Key}:{string.Join(",", x.Value.Select(v => v.Replace("\r\n", string.Empty)))}\n"));
		}

		public string GetCanonicalizedResource(StorageType storageType, Uri address, string accountName)
		{
			var queryString = new Dictionary<string, string>();

			if (storageType != StorageType.Table)
			{
				var queryStringValues = QueryHelpers.ParseQuery(address.Query);

				foreach (string key in queryStringValues.Keys)
				{
					queryString.Add(key?.ToLowerInvariant(), string.Join(",", queryStringValues[key].OrderBy(x => x)));
				}
			}

			return $"/{accountName}{address.AbsolutePath}{string.Join(string.Empty, queryString.OrderBy(x => x.Key).Select(x => $"\n{x.Key}:{x.Value}"))}";
		}

		private string GetBaseUri(StorageType storageType)
		{
			return $"https://{StorageAccount}.{storageType.ToString().ToLower()}.core.windows.net/";
		}
	}
}
