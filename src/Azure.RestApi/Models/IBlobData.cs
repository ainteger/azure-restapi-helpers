namespace Azure.RestApi.Models
{
	public interface IBlobData
	{
		string Name { get; }
		string Url { get; }
		string ContentType { get; }
		string ContentLength { get; }
	}
}
