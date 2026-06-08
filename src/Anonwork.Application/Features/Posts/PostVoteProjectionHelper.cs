using Anonwork.Application.Features.Posts.DTOs.Response;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Posts;

internal static class PostVoteProjectionHelper
{
    public static PostResponseDto MapToResponse(Post post, bool isUpvotedByMe)
    {
        var imageUrls = post.PostImages
            .OrderBy(pi => pi.DisplayOrder)
            .Select(pi => pi.ImageUrl)
            .ToList();

        return new PostResponseDto(
            Id: post.Id,
            Title: post.Title,
            Content: post.Content,
            AuthorId: post.AuthorId,
            AuthorUsername: post.Author?.Username,
            AuthorAnonAlias: post.Author?.AnonAlias,
            IsAnonymous: post.IsAnonymous,
            SubjectId: post.SubjectId,
            SubjectName: post.Subject?.Name,
            ImageUrls: imageUrls,
            RemainingImagesCount: 0,
            Tags: post.PostTags.Select(pt => pt.Tag).ToList(),
            Upvotes: post.Upvotes,
            CommentsCount: post.CommentsCount,
            ViewCount: post.ViewCount,
            Status: post.Status.ToString(),
            CreatedAt: post.CreatedAt,
            UpdatedAt: post.UpdatedAt,
            IsUpvotedByMe: isUpvotedByMe
        );
    }
}
