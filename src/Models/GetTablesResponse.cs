using System.Text.Json.Serialization;

namespace Azure.RestApi.Models
{
	public class GetTablesResponse
	{
		[JsonPropertyName("value")]
		public IEnumerable<GetTableResponse>? Value { get; set; }

		public class GetTableResponse
		{
			[JsonPropertyName("TableName")]
			public string? TableName { get; set; }
		}
	}
}
