using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.PostRatings.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Application.Features.PostRatings;

public class GetPostRatingSummaryUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Post> _postRepo = unitOfWork.GetRepository<Post>();
    private readonly IGenericRepository<PostRating> _ratingRepo = unitOfWork.GetRepository<PostRating>();

    public async Task<PostRatingSummaryDto> ExecuteAsync(
        Guid postId,
        Guid? currentUserId = null,
        CancellationToken ct = default)
    {
        if (postId == Guid.Empty)
            throw new ArgumentException("Post ID is required.");

        var post = await _postRepo.GetQueryableNoTracking()
            .FirstOrDefaultAsync(p => p.Id == postId && p.Status == PostStatus.Published, ct);

        if (post is null)
            throw new NotFoundException(nameof(Post), postId);

        var ratingsQuery = _ratingRepo.GetQueryableNoTracking()
            .Include(r => r.User)
            .Where(r => r.PostId == postId);

        var ratingsList = await ratingsQuery
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

        var breakdown = new Dictionary<int, int>
        {
            { 5, ratingsList.Count(r => r.Stars == 5) },
            { 4, ratingsList.Count(r => r.Stars == 4) },
            { 3, ratingsList.Count(r => r.Stars == 3) },
            { 2, ratingsList.Count(r => r.Stars == 2) },
            { 1, ratingsList.Count(r => r.Stars == 1) }
        };

        PostRatingItemDto? myRatingDto = null;
        if (currentUserId.HasValue && currentUserId.Value != Guid.Empty)
        {
            var myRating = ratingsList.FirstOrDefault(r => r.UserId == currentUserId.Value);
            if (myRating is not null)
            {
                myRatingDto = new PostRatingItemDto(
                    Id: myRating.Id,
                    UserId: myRating.UserId,
                    AuthorName: myRating.User.AnonAlias ?? myRating.User.Username,
                    Stars: myRating.Stars,
                    Review: myRating.Review,
                    CreatedAt: myRating.CreatedAt,
                    UpdatedAt: myRating.UpdatedAt
                );
            }
        }

        var recentRatings = ratingsList
            .Take(10)
            .Select(r => new PostRatingItemDto(
                Id: r.Id,
                UserId: r.UserId,
                AuthorName: r.User.AnonAlias ?? r.User.Username,
                Stars: r.Stars,
                Review: r.Review,
                CreatedAt: r.CreatedAt,
                UpdatedAt: r.UpdatedAt
            ))
            .ToList();

        return new PostRatingSummaryDto(
            PostId: post.Id,
            AverageRating: post.AverageRating,
            RatingsCount: post.RatingsCount,
            QualityScore: post.QualityScore,
            StarBreakdown: breakdown,
            MyRating: myRatingDto,
            RecentRatings: recentRatings
        );
    }
}
