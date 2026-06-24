using Anonwork.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Anonwork.Application.Interfaces;

public interface IPostMediaService
{
    Task<List<PostMedia>> BuildPostMediaAsync(
        Guid postId,
        IFormFileCollection? images,
        IFormFileCollection? files,
        CancellationToken cancellationToken = default);

    Task<List<PostMedia>> AppendPostMediaAsync(
        Guid postId,
        IEnumerable<PostMedia> existingMedia,
        IFormFileCollection? images,
        IFormFileCollection? files,
        CancellationToken cancellationToken = default);

    Task RemoveMediaFilesAsync(IEnumerable<PostMedia> mediaItems, CancellationToken cancellationToken = default);
}
