namespace Azure.RestApi.Models
{
	public class BlobData : IBlobData
	{
		public string Name { get; set; }
		public string ContentType { get; set; }
		public string ContentLength { get; set; }
	}
}
