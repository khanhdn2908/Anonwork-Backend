using Anonwork.Application.Features.Posts.DTOs.Response;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Posts.Helpers;

public static class PostVoteProjectionHelper
{
    public static PostResponseDto MapToResponse(Post post, bool isUpvotedByMe)
    {
        var isAnon = post.IsAnonymous && post.Author.IsAnonDefault;
        var imageUrls = post.PostImages
            .OrderBy(pi => pi.DisplayOrder)
            .Select(pi => pi.ImageUrl)
            .ToList();

        return new PostResponseDto(
            post.Id,
            post.Title,
            post.Content,
            post.AuthorId,
            isAnon ? null : post.Author.Username,
            isAnon ? post.Author.AnonAlias : null,
            isAnon,
            post.SubjectId,
            post.Subject?.Name,
            imageUrls,
            0,
            post.PostTags.Select(pt => pt.Tag).ToList(),
            post.Upvotes,
            post.CommentsCount,
            post.ViewCount,
            post.Status.ToString(),
            post.CreatedAt,
            post.UpdatedAt,
            isUpvotedByMe
        );
    }
}
