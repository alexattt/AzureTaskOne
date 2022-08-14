namespace TaskApi.Services
{
    public interface IBlobStorageInterface
    {
        Task<string> GetBlobContent(string blobGUID);
    }
}
