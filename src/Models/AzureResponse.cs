using System.Net;
using System.Linq;
using System.Net.Http;

namespace Azure.RestApi.Models
{
	public class AzureResponse
	{
		public bool IsSuccess { get; }
		public HttpStatusCode StatusCode { get; }
		public string ErrorCode { get; }

		public AzureResponse(HttpResponseMessage response)
		{
			IsSuccess = response.IsSuccessStatusCode;
			StatusCode = response.StatusCode;
			ErrorCode = response.Headers.Contains("x-ms-error-code") ? response.Headers?.GetValues("x-ms-error-code")?.FirstOrDefault() : null;
		}
	}
}