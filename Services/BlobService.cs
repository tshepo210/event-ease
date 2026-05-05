using Azure.Storage.Blobs;

public class BlobService
{
    private readonly string connectionString = "YOUR_CONNECTION_STRING";

    public async Task<string> UploadAsync(IFormFile file)
    {
        var container = new BlobContainerClient(connectionString, "images");
        await container.CreateIfNotExistsAsync();

        var blob = container.GetBlobClient(file.FileName);

        using var stream = file.OpenReadStream();
        await blob.UploadAsync(stream, true);

        return blob.Uri.ToString();
    }
}