using Newtonsoft.Json;

namespace Azure.RestApi.Models
{
	public class CreateTableRequest
	{
		[JsonProperty("TableName")]
		public string TableName { get; set; }
	}
}
