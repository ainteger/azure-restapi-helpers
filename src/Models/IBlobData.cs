namespace Azure.RestApi.Models
{
	public interface IBlobData
	{
		string Name { get; }
		string ContentType { get; }
		string ContentLength { get; }
	}
}
