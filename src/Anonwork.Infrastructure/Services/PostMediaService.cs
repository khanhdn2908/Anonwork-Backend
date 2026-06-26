using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Anonwork.Infrastructure.Services;

public class PostMediaService(IR2Service r2Service, IPlanAccessService planAccessService) : IPostMediaService
{
    private readonly IR2Service _r2Service = r2Service;
    private readonly IPlanAccessService _planAccessService = planAccessService;

    public async Task<List<PostMedia>> BuildPostMediaAsync(
        Guid postId,
        IFormFileCollection? images,
        IFormFileCollection? files,
        CancellationToken cancellationToken = default)
    {
        return await AppendPostMediaAsync(postId, [], images, files, cancellationToken);
    }

    public async Task<List<PostMedia>> AppendPostMediaAsync(
        Guid postId,
        IEnumerable<PostMedia> existingMedia,
        IFormFileCollection? images,
        IFormFileCollection? files,
        CancellationToken cancellationToken = default)
    {
        var media = existingMedia.OrderBy(m => m.DisplayOrder).ToList();
        var uploadedKeys = new List<string>();
        var now = DateTime.UtcNow;
        var nextDisplayOrder = media.Count;

        try
        {
            if (images is not null && images.Count > 0)
            {
                foreach (var image in images.Take(5))
                {
                    ValidateImageFile(image);
                    var (fileKey, _) = await _r2Service.UploadFileAsync(image, $"posts/{postId}/images", cancellationToken);
                    uploadedKeys.Add(fileKey);

                    media.Add(new PostMedia
                    {
                        Id = Guid.NewGuid(),
                        PostId = postId,
                        MediaType = PostMediaType.Image,
                        FileKey = fileKey,
                        ContentType = image.ContentType,
                        FileSize = image.Length,
                        OriginalFileName = image.FileName,
                        DisplayOrder = nextDisplayOrder++,
                        CreatedAt = now
                    });
                }
            }

            if (files is not null && files.Count > 0)
            {
                foreach (var file in files.Take(5))
                {
                    ValidateDocumentFile(file);
                    var (fileKey, _) = await _r2Service.UploadFileAsync(file, $"posts/{postId}/files", cancellationToken);
                    uploadedKeys.Add(fileKey);

                    media.Add(new PostMedia
                    {
                        Id = Guid.NewGuid(),
                        PostId = postId,
                        MediaType = PostMediaType.File,
                        FileKey = fileKey,
                        ContentType = file.ContentType,
                        FileSize = file.Length,
                        OriginalFileName = file.FileName,
                        DisplayOrder = nextDisplayOrder++,
                        CreatedAt = now
                    });
                }
            }

            return media;
        }
        catch
        {
            await RollbackUploadedFilesAsync(uploadedKeys, cancellationToken);
            throw;
        }
    }

    public async Task RemoveMediaFilesAsync(IEnumerable<PostMedia> mediaItems, CancellationToken cancellationToken = default)
    {
        foreach (var media in mediaItems)
            await _r2Service.DeleteFileAsync(media.FileKey, cancellationToken);
    }

    private async Task RollbackUploadedFilesAsync(IEnumerable<string> fileKeys, CancellationToken cancellationToken)
    {
        foreach (var fileKey in fileKeys.Distinct())
            await _r2Service.DeleteFileAsync(fileKey, cancellationToken);
    }

    private static void ValidateImageFile(IFormFile file)
    {
        var contentType = file.ContentType?.ToLowerInvariant() ?? string.Empty;
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!contentType.StartsWith("image/") &&
            extension is not ".jpg" and not ".jpeg" and not ".png" and not ".gif" and not ".webp" and not ".bmp" and not ".svg" and not ".ico")
        {
            throw new ArgumentException($"File '{file.FileName}' is not a valid image.");
        }
    }

    private static void ValidateDocumentFile(IFormFile file)
    {
        var contentType = file.ContentType?.ToLowerInvariant() ?? string.Empty;
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (contentType.StartsWith("image/") ||
            extension is ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp" or ".bmp" or ".svg" or ".ico")
        {
            throw new ArgumentException($"File '{file.FileName}' must be a document or attachment, not an image.");
        }
    }
}
