using System.Threading.Tasks;

namespace Azure.RestApi
{
    public interface IBlob
    {
        Task<bool> PutBlobAsync(string container, string contentName, byte[] content);
        Task<byte[]> GetBlobOrDefaultAsync(string container, string contentName);
        Task<bool> DeleteBlobAsync(string container, string contentName);
    }
}
