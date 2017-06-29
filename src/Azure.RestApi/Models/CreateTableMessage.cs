using Newtonsoft.Json;

namespace Azure.RestApi.Models
{
    public class CreateTableMessage
    {
        [JsonProperty("TableName")]
        public string TableName { get; set; }
    }
}
