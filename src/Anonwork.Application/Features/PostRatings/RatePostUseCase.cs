using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.PostRatings.DTOs.Requests;
using Anonwork.Application.Features.PostRatings.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Anonwork.Application.Features.PostRatings;

public class RatePostUseCase(IUnitOfWork unitOfWork, IDistributedCache? cache = null)
{
    private readonly IGenericRepository<Post> _postRepo = unitOfWork.GetRepository<Post>();
    private readonly IGenericRepository<PostRating> _ratingRepo = unitOfWork.GetRepository<PostRating>();

    public async Task<PostRatingResponseDto> ExecuteAsync(
        Guid currentUserId,
        Guid postId,
        RatePostRequestDto request,
        CancellationToken ct = default)
    {
        if (currentUserId == Guid.Empty)
            throw new ArgumentException("Current user ID is required.");

        if (postId == Guid.Empty)
            throw new ArgumentException("Post ID is required.");

        if (request.Stars < 1 || request.Stars > 5)
            throw new ArgumentOutOfRangeException(nameof(request.Stars), "Stars must be between 1 and 5.");

        var post = await _postRepo.GetQueryable()
            .FirstOrDefaultAsync(p => p.Id == postId && p.Status == PostStatus.Published, ct);

        if (post is null)
            throw new NotFoundException(nameof(Post), postId);

        var existingRating = await _ratingRepo.GetQueryable()
            .FirstOrDefaultAsync(r => r.UserId == currentUserId && r.PostId == postId, ct);

        string message;
        if (existingRating is null)
        {
            var newRating = new PostRating
            {
                Id = Guid.NewGuid(),
                PostId = postId,
                UserId = currentUserId,
                Stars = request.Stars,
                Review = request.Review?.Trim(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _ratingRepo.AddAsync(newRating, ct);
            message = "Bài viết đã được chấm điểm thành công.";
        }
        else
        {
            existingRating.Stars = request.Stars;
            existingRating.Review = request.Review?.Trim();
            existingRating.UpdatedAt = DateTime.UtcNow;
            await _ratingRepo.UpdateAsync(existingRating, ct);
            message = "Đánh giá của bạn đã được cập nhật.";
        }

        await unitOfWork.SaveChangesAsync(ct);

        // Recalculate metrics
        await RecalculatePostRatingMetricsAsync(post, ct);

        if (cache is not null)
        {
            try
            {
                await cache.RemoveAsync($"post:{postId}", ct);
            }
            catch
            {
                // Ignore cache error in development
            }
        }

        return new PostRatingResponseDto(
            PostId: post.Id,
            AverageRating: post.AverageRating,
            RatingsCount: post.RatingsCount,
            QualityScore: post.QualityScore,
            MyStars: request.Stars,
            MyReview: request.Review?.Trim(),
            Message: message
        );
    }

    private async Task RecalculatePostRatingMetricsAsync(Post post, CancellationToken ct)
    {
        var ratings = await _ratingRepo.GetQueryableNoTracking()
            .Where(r => r.PostId == post.Id)
            .ToListAsync(ct);

        int count = ratings.Count;
        decimal avg = count > 0 ? Math.Round((decimal)ratings.Average(r => r.Stars), 2) : 0m;

        double avgDouble = (double)avg;
        double qualityScore = (avgDouble * 0.5) + (Math.Log10(post.Upvotes + 1) * 0.3) + (Math.Log10(post.CommentsCount + 1) * 0.2);

        post.AverageRating = avg;
        post.RatingsCount = count;
        post.QualityScore = Math.Round(qualityScore, 4);
        post.UpdatedAt = DateTime.UtcNow;

        await _postRepo.UpdateAsync(post, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
