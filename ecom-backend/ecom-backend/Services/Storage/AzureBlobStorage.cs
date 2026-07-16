using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ecom_backend.Interfaces.Services;

namespace ecom_backend.Services.Storage;

/// <summary>
/// Stores files in Azure Blob Storage. Intended for production.
/// Requires Storage:AzureBlob:ConnectionString and :Container config.
/// </summary>
public class AzureBlobStorage : IFileStorage
{
    private readonly BlobContainerClient _container;
    private readonly ILogger<AzureBlobStorage> _logger;

    public AzureBlobStorage(IConfiguration configuration, ILogger<AzureBlobStorage> logger)
    {
        _logger = logger;

        var connectionString = configuration["Storage:AzureBlob:ConnectionString"]
            ?? throw new InvalidOperationException(
                "Storage:AzureBlob:ConnectionString is not configured.");
        var containerName = configuration["Storage:AzureBlob:Container"] ?? "media";

        _container = new BlobContainerClient(connectionString, containerName);
        _container.CreateIfNotExists(PublicAccessType.Blob);
    }

    public async Task<string> SaveAsync(
        Stream content,
        string originalFileName,
        string contentType,
        string folder,
        CancellationToken cancellationToken = default)
    {
        var blobName = $"{folder}/{Guid.NewGuid():N}{GetExtension(originalFileName, contentType)}";
        var blob = _container.GetBlobClient(blobName);

        await blob.UploadAsync(
            content,
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
            },
            cancellationToken);

        return blob.Uri.ToString();
    }

    public async Task DeleteAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                return;

            var uri = new Uri(fileUrl);
            var prefix = $"/{_container.Name}/";
            var path = uri.AbsolutePath;
            var index = path.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);
            if (index < 0)
                return;

            var blobName = path[(index + prefix.Length)..];
            await _container.GetBlobClient(blobName).DeleteIfExistsAsync(cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete blob {FileUrl}", fileUrl);
        }
    }

    private static string GetExtension(string originalFileName, string contentType)
    {
        var ext = Path.GetExtension(originalFileName);
        if (!string.IsNullOrWhiteSpace(ext))
            return ext.ToLowerInvariant();

        return contentType switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            "image/gif" => ".gif",
            _ => string.Empty
        };
    }
}
