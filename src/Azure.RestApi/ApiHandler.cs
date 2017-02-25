using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Azure.RestApi
{
    internal class ApiHandler
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
            byte[] byteArray = null;
            var now = DateTime.UtcNow;
            var uri = Endpoint + resource;

            var request = new HttpRequestMessage(method, uri);

            request.Headers.Add("x-ms-date", now.ToString("R", System.Globalization.CultureInfo.InvariantCulture));
            request.Headers.Add("x-ms-version", "2009-09-19");

            //TODO: Table storage
            //if (IsTableStorage)
            //{
            //    request.ContentType = "application/atom+xml";

            //    request.Headers.Add("DataServiceVersion", "1.0;NetFx");
            //    request.Headers.Add("MaxDataServiceVersion", "1.0;NetFx");
            //}

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            if (!string.IsNullOrEmpty(requestBody))
            {
                request.Headers.Add("Accept-Charset", "UTF-8");
                byteArray = Encoding.UTF8.GetBytes(requestBody);
                request.Content = new ByteArrayContent(byteArray);
                request.Content.Headers.Add("Content-Length", byteArray.Length.ToString());
            }

            request.Headers.Add("Authorization", GetAuthorizationHeader(method, now, request, ifMatch, md5));

            return request;
        }

        private string GetAuthorizationHeader(HttpMethod method, DateTime now, HttpRequestMessage request, string ifMatch = "", string md5 = "")
        {
            string messageSignature;

            if (IsTableStorage)
            {
                messageSignature = string.Format("{0}\n\n{1}\n{2}\n{3}",
                    method,
                    "application/atom+xml",
                    now.ToString("R", System.Globalization.CultureInfo.InvariantCulture),
                    GetCanonicalizedResource(request.RequestUri, StorageAccount)
                    );
            }
            else
            {
                messageSignature = string.Format("{0}\n\n\n{1}\n{5}\n\n\n\n{2}\n\n\n\n{3}{4}",
                    method,
                    (method == HttpMethod.Get || method == HttpMethod.Head) ? string.Empty : request.Content.Headers.First(x => x.Key == "Content-Length").Value.First(),
                    ifMatch,
                    GetCanonicalizedHeaders(request),
                    GetCanonicalizedResource(request.RequestUri, StorageAccount),
                    md5
                    );
            }

            var SignatureBytes = Encoding.UTF8.GetBytes(messageSignature);
            var SHA256 = new System.Security.Cryptography.HMACSHA256(Convert.FromBase64String(StorageKey));
            var AuthorizationHeader = "SharedKey " + StorageAccount + ":" + Convert.ToBase64String(SHA256.ComputeHash(SignatureBytes));
            return AuthorizationHeader;
        }

        private string GetCanonicalizedHeaders(HttpRequestMessage request)
        {
            var sortedHeaders = request.Headers.Where(x => x.Key.ToLowerInvariant().StartsWith("x-ms-", StringComparison.Ordinal))
                .OrderBy(x => x.Key);

            return string.Join(string.Empty,
                sortedHeaders.Select(x =>
                    $"{x.Key}:{string.Join(",", x.Value.Select(v => v.Replace("\r\n", string.Empty)))}\n"));
        }

        private string GetCanonicalizedResource(Uri address, string accountName)
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
