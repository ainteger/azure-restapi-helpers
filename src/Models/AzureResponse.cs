using System.Net;
using System.Linq;
using System.Net.Http;

namespace Azure.RestApi.Models
{
	public class AzureResponse
	{
		public static AzureResponse Parse(HttpResponseMessage response)
		{
			return new AzureResponse
			{
				IsSuccess = response.IsSuccessStatusCode,
				StatusCode = response.StatusCode,
				ErrorCode = response.Headers.Contains("x-ms-error-code") ? response.Headers?.GetValues("x-ms-error-code")?.FirstOrDefault() : null
			};
		}

		public bool IsSuccess { get; set; }
		public HttpStatusCode StatusCode { get; set; }
		public string ErrorCode { get; set; }
	}
}