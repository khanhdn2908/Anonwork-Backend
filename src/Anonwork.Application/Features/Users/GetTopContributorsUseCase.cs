using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Anonwork.Application.Features.Users.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Application.Features.Users;

public class GetTopContributorsUseCase(IUnitOfWork unitOfWork, IR2Service r2Service)
{
    private readonly IGenericRepository<User> _userRepo = unitOfWork.GetRepository<User>();
    private readonly IR2Service _r2Service = r2Service;

    public async Task<TopContributorsListResponseDto> ExecuteAsync(
        int? month = null,
        int? year = null,
        int limit = 10,
        CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        int targetMonth = month.HasValue && month.Value >= 1 && month.Value <= 12 ? month.Value : now.Month;
        int targetYear = year.HasValue && year.Value >= 2020 && year.Value <= 2100 ? year.Value : now.Year;

        if (limit < 1) limit = 10;
        if (limit > 50) limit = 50;

        var startOfMonth = new DateTime(targetYear, targetMonth, 1, 0, 0, 0, DateTimeKind.Utc);
        var endOfMonth = startOfMonth.AddMonths(1);

        var usersWithActivity = await _userRepo.GetQueryableNoTracking()
            .Include(u => u.AnonImage)
            .Include(u => u.Posts.Where(p => p.CreatedAt >= startOfMonth && p.CreatedAt < endOfMonth && p.Status == PostStatus.Published))
            .Include(u => u.Comments.Where(c => c.CreatedAt >= startOfMonth && c.CreatedAt < endOfMonth))
            .Where(u => u.Status == UserStatus.Active && (
                u.Posts.Any(p => p.CreatedAt >= startOfMonth && p.CreatedAt < endOfMonth && p.Status == PostStatus.Published) ||
                u.Comments.Any(c => c.CreatedAt >= startOfMonth && c.CreatedAt < endOfMonth)
            ))
            .ToListAsync(ct);

        var rankedList = usersWithActivity
            .Select(u =>
            {
                int postsCount = u.Posts.Count;
                int commentsCount = u.Comments.Count;
                int upvotesReceived = u.Posts.Sum(p => p.Upvotes);

                var ratedPosts = u.Posts.Where(p => p.RatingsCount > 0).ToList();
                decimal avgRating = ratedPosts.Count > 0
                    ? Math.Round(ratedPosts.Average(p => p.AverageRating), 2)
                    : 0m;

                double avgRatingDouble = (double)avgRating;
                double contributionScore = (postsCount * 10) + (upvotesReceived * 3) + (avgRatingDouble * 5) + (commentsCount * 2);

                var isAnon = u.IsAnonDefault;
                var avatarUrl = isAnon
                    ? (!string.IsNullOrWhiteSpace(u.AnonImage?.FileKey)
                        ? _r2Service.GetPublicUrl(u.AnonImage.FileKey)
                        : _r2Service.GetPublicUrl("avatars/null.jpg"))
                    : (string.IsNullOrWhiteSpace(u.AvatarKey)
                        ? _r2Service.GetPublicUrl("avatars/null.jpg")
                        : _r2Service.GetPublicUrl(u.AvatarKey));

                return new
                {
                    User = u,
                    DisplayName = isAnon ? u.AnonAlias : u.Username,
                    AvatarUrl = avatarUrl,
                    IsAnonymous = isAnon,
                    PostsCount = postsCount,
                    CommentsCount = commentsCount,
                    UpvotesReceived = upvotesReceived,
                    AverageRating = avgRating,
                    ContributionScore = Math.Round(contributionScore, 2)
                };
            })
            .Where(x => x.ContributionScore > 0)
            .OrderByDescending(x => x.ContributionScore)
            .ThenByDescending(x => x.PostsCount)
            .ThenByDescending(x => x.UpvotesReceived)
            .Take(limit)
            .Select((x, index) => new TopContributorDto(
                Rank: index + 1,
                UserId: x.User.Id,
                Username: x.IsAnonymous ? x.DisplayName : x.User.Username,
                DisplayName: x.DisplayName,
                AvatarUrl: x.AvatarUrl,
                IsAnonymous: x.IsAnonymous,
                PostsCount: x.PostsCount,
                CommentsCount: x.CommentsCount,
                UpvotesReceived: x.UpvotesReceived,
                AverageRating: x.AverageRating,
                ContributionScore: x.ContributionScore
            ))
            .ToList();

        return new TopContributorsListResponseDto(
            Month: targetMonth,
            Year: targetYear,
            Contributors: rankedList
        );
    }
}
