using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;

namespace Anonwork.Application.Features.Posts;

public class DeletePostUseCasePermanent(IUnitOfWork unitOfWork, IPostMediaService postMediaService, IAppDbContext dbContext)
{
    private readonly IGenericRepository<Post> _postRepo = unitOfWork.GetRepository<Post>();
    private readonly IPostMediaService _postMediaService = postMediaService;
    private readonly IAppDbContext _dbContext = dbContext;

    public async Task ExecuteAsync(Guid postId, CancellationToken ct = default)
    {
        if (postId == Guid.Empty)
            throw new ArgumentException("Post id is required.");

        var post = await _postRepo.FindSingleWithTrackingAsync(p => p.Id == postId, ct)
            ?? throw new NotFoundException(nameof(Post), postId);

        if (post.Status != PostStatus.Deleted)
            throw new ArgumentException("Post need deleted first.");

        var mediaItems = post.PostMediaItems.ToList();

        await using var transaction = await _dbContext.BeginTransactionAsync(ct);
        try
        {
            await _postMediaService.RemoveMediaFilesAsync(mediaItems, ct);
            await _postRepo.DeleteAsync(postId, ct);
            await unitOfWork.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}
