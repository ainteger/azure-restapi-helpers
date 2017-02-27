using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Azure.RestApi
{
    /// <summary>
    /// Handles common api things
    /// </summary>
    public class ApiHandler
    {
        private string StorageAccount { get; }
        private string StorageKey { get; }
        private string Endpoint { get; }
        private bool IsTableStorage { get; }

        public ApiHandler(string storageAccount, string storageKey, string azureFunction, bool isTableStorage)
        {
            StorageAccount = storageAccount;
            StorageKey = storageKey;
            Endpoint = $"https://{storageAccount}.{azureFunction}.core.windows.net/";
            IsTableStorage = isTableStorage;
        }

        public HttpRequestMessage GetRequest(
            HttpMethod method,
            string resource,
            string requestBody = null,
            SortedList<string, string> headers = null,
            string ifMatch = "",
            string md5 = "")
        {
            var now = DateTime.UtcNow;

            var request = new HttpRequestMessage(method, Endpoint + resource);

            request.Headers.Add("x-ms-date", now.ToString("R", System.Globalization.CultureInfo.InvariantCulture));

            if (IsTableStorage)
            {
                request.Headers.Add("x-ms-version", "2016-05-31");
                request.Headers.Add("DataServiceVersion", "3.0;NetFx");
                request.Headers.Add("MaxDataServiceVersion", "3.0;NetFx");
                request.Headers.Add("Accept", "application/json");
            }
            else
            {
                request.Headers.Add("x-ms-version", "2009-09-19");
            }

            foreach (var header in headers ?? new SortedList<string, string>())
            {
                request.Headers.Add(header.Key, header.Value);
            }

            if (!string.IsNullOrEmpty(requestBody))
            {
                request.Headers.Add("Accept-Charset", "UTF-8");
                var byteArray = Encoding.UTF8.GetBytes(requestBody);
                request.Content = new ByteArrayContent(byteArray);
                request.Content.Headers.Add("Content-Length", byteArray.Length.ToString());

                if (IsTableStorage)
                {
                    request.Content.Headers.Add("Content-Type", "application/json");
                }
            }

            if (!string.IsNullOrWhiteSpace(ifMatch))
            {
                request.Headers.Add("If-Match", ifMatch);
            }

            request.Headers.Add("Authorization", GetAuthorizationHeader(method, now, request, ifMatch, md5));

            return request;
        }

        public string GetAuthorizationHeader(HttpMethod method, DateTime now, HttpRequestMessage request, string ifMatch = "", string md5 = "")
        {
            if (IsTableStorage)
            {
                var messageSignature = string.Format("{0}\n{1}",
                    now.ToString("R", System.Globalization.CultureInfo.InvariantCulture),
                    $"/{StorageAccount}/{request.RequestUri.AbsolutePath.TrimStart('/')}");

                return $"SharedKeyLite {StorageAccount}:{GetSignature(messageSignature)}";
            }
            else
            {
                var messageSignature = string.Format("{0}\n\n\n{1}\n{5}\n\n\n\n{2}\n\n\n\n{3}{4}",
                    method,
                    (method == HttpMethod.Get || method == HttpMethod.Head) ? string.Empty : request.Content?.Headers?.FirstOrDefault(x => x.Key == "Content-Length").Value.FirstOrDefault() ?? string.Empty,
                    ifMatch,
                    GetCanonicalizedHeaders(request),
                    GetCanonicalizedResource(request.RequestUri, StorageAccount),
                    md5
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

        public string GetCanonicalizedResource(Uri address, string accountName)
        {
            var queryString = new Dictionary<string, string>();

            if (!IsTableStorage)
            {
                var queryStringValues = QueryHelpers.ParseQuery(address.Query);

                foreach (string key in queryStringValues.Keys)
                {
                    queryString.Add(key?.ToLowerInvariant(), string.Join(",", queryStringValues[key].OrderBy(x => x)));
                }
            }

            return $"/{accountName}{address.AbsolutePath}{string.Join(string.Empty, queryString.OrderBy(x => x.Key).Select(x => $"\n{x.Key}:{x.Value}"))}";
        }
    }
}
