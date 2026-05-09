using Azure.Storage.Blobs;

namespace EventEase.Services
{
    public class BlobService
    {
        private readonly IConfiguration configuration;

        public BlobService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> UploadAsync(IFormFile file)
        {
            var connectionString = configuration.GetConnectionString("AzureStorage");

            var containerClient = new BlobContainerClient(connectionString, "venue-images");

            await containerClient.CreateIfNotExistsAsync();

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

            var blobClient = containerClient.GetBlobClient(fileName);

            using var stream = file.OpenReadStream();

            await blobClient.UploadAsync(stream, true);

            return blobClient.Uri.ToString();
        }
    }
}