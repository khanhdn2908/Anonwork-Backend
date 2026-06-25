using Microsoft.AspNetCore.Http;

namespace Anonwork.Application.Interfaces;

public interface IR2Service
{
    Task<(string FileKey, string FileUrl)> UploadFileAsync(IFormFile file, string folder, CancellationToken cancellationToken = default);

    Task<(string FileKey, string FileUrl)> UploadFileAsync(Stream stream, string fileName, string contentType, string folder, CancellationToken cancellationToken = default);

    Task<bool> DeleteFileAsync(string fileKey, CancellationToken cancellationToken = default);

    string GetPublicUrl(string fileKey);

    string GetDefaultAvatarKey();
}
