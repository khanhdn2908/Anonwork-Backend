using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;

namespace Anonwork.Application.Features.Posts;

public class DeletePostUseCasePermanent(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Post> _postRepo = unitOfWork.GetRepository<Post>();

    public async Task ExecuteAsync(Guid postId, CancellationToken ct = default)
    {
        if (postId == Guid.Empty)
            throw new ArgumentException("Post id is required.");

        var post = await _postRepo.GetByIdAsync(postId, ct)
            ?? throw new NotFoundException(nameof(Post), postId);

        if (post.Status != PostStatus.Deleted)
            throw new ArgumentException("Post need deleted first.");

        await _postRepo.DeleteAsync(postId, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
