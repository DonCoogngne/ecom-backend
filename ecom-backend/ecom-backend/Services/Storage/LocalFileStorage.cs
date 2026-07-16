using ecom_backend.Interfaces.Services;

namespace ecom_backend.Services.Storage;

/// <summary>
/// Stores files under the web root (wwwroot/uploads/...) and serves them via
/// static files. Intended for local development.
/// </summary>
public class LocalFileStorage(
    IWebHostEnvironment environment,
    IHttpContextAccessor httpContextAccessor,
    IConfiguration configuration,
    ILogger<LocalFileStorage> logger) : IFileStorage
{
    private const string UploadsRoot = "uploads";

    public async Task<string> SaveAsync(
        Stream content,
        string originalFileName,
        string contentType,
        string folder,
        CancellationToken cancellationToken = default)
    {
        var webRoot = GetWebRoot();
        var relativeFolder = Path.Combine(UploadsRoot, folder);
        var absoluteFolder = Path.Combine(webRoot, relativeFolder);
        Directory.CreateDirectory(absoluteFolder);

        var fileName = $"{Guid.NewGuid():N}{GetExtension(originalFileName, contentType)}";
        var absolutePath = Path.Combine(absoluteFolder, fileName);

        await using (var fileStream = new FileStream(absolutePath, FileMode.Create, FileAccess.Write))
        {
            await content.CopyToAsync(fileStream, cancellationToken);
        }

        // URL path always uses forward slashes.
        var urlPath = $"/{UploadsRoot}/{folder}/{fileName}";
        return $"{GetPublicBaseUrl()}{urlPath}";
    }

    public Task DeleteAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                return Task.CompletedTask;

            var marker = $"/{UploadsRoot}/";
            var index = fileUrl.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (index < 0)
                return Task.CompletedTask;

            var relative = fileUrl[(index + 1)..].Replace('/', Path.DirectorySeparatorChar);
            var absolutePath = Path.Combine(GetWebRoot(), relative);
            if (File.Exists(absolutePath))
                File.Delete(absolutePath);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to delete local file {FileUrl}", fileUrl);
        }

        return Task.CompletedTask;
    }

    private string GetWebRoot()
    {
        var webRoot = environment.WebRootPath
            ?? Path.Combine(environment.ContentRootPath, "wwwroot");
        Directory.CreateDirectory(webRoot);
        return webRoot;
    }

    private string GetPublicBaseUrl()
    {
        var configured = configuration["Storage:Local:PublicBaseUrl"];
        if (!string.IsNullOrWhiteSpace(configured))
            return configured.TrimEnd('/');

        var request = httpContextAccessor.HttpContext?.Request;
        if (request is not null)
            return $"{request.Scheme}://{request.Host}";

        return string.Empty;
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
