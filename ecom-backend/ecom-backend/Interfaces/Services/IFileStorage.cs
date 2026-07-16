namespace ecom_backend.Interfaces.Services;

/// <summary>
/// Abstraction over a file/blob store. Local disk is used in development;
/// Azure Blob Storage is used in production (selected via configuration).
/// </summary>
public interface IFileStorage
{
    /// <summary>
    /// Persists the content and returns a publicly reachable URL.
    /// </summary>
    /// <param name="content">File content stream.</param>
    /// <param name="originalFileName">Original name, used only to derive the extension.</param>
    /// <param name="contentType">MIME type of the content.</param>
    /// <param name="folder">Logical folder/prefix (e.g. "avatars").</param>
    Task<string> SaveAsync(
        Stream content,
        string originalFileName,
        string contentType,
        string folder,
        CancellationToken cancellationToken = default);

    /// <summary>Deletes a previously stored file by its public URL. No-op if not found.</summary>
    Task DeleteAsync(string fileUrl, CancellationToken cancellationToken = default);
}
