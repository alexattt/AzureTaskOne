using Azure.Storage.Blobs;

namespace TaskApi.Services
{
    public class BlobStorageService : IBlobStorageInterface
    {
        private const string TableName = "apiblobdata";
        private readonly IConfiguration _configuration;

        public BlobStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private async Task<BlobContainerClient> GetBlobContainerClient()
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_configuration["AzureStorageConn"]);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(TableName);

            return blobContainerClient;
        }

        public async Task<string> GetBlobContent(string blobGUID)
        {
            string blobContent = "";

            var blobContainerClient = await GetBlobContainerClient();
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobGUID);

            var response = await blobClient.DownloadAsync();
            using (var streamReader = new StreamReader(response.Value.Content))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = await streamReader.ReadLineAsync();
                    blobContent = blobContent + line;
                }
            }

            return blobContent;
        }
    }
}
