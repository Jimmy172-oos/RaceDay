using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace RaceDay.Api.Services
{
    public class BlobStorageService
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public BlobStorageService(IConfiguration config)
        {
            _connectionString = config["AzureStorage:ConnectionString"]!;
            _containerName = config["AzureStorage:ContainerName"]!;
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var blobClient = blobContainerClient.GetBlobClient(fileName);

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });

            return blobClient.Uri.ToString();
        }
    }
}