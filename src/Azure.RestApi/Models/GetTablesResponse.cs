using Newtonsoft.Json;
using System.Collections.Generic;

namespace Azure.RestApi.Models
{
	public class GetTablesResponse
	{
		[JsonProperty("value")]
		public IEnumerable<GetTableResponse> Value { get; set; }

		public class GetTableResponse
		{
			[JsonProperty("TableName")]
			public string TableName { get; set; }
		}
	}
}
