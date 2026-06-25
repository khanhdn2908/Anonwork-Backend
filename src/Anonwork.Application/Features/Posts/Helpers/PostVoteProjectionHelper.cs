using Anonwork.Application.Features.Posts.DTOs.Response;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Posts.Helpers;

public static class PostVoteProjectionHelper
{
    public static PostResponseDto MapToResponse(Post post, bool isUpvotedByMe, IR2Service r2Service)
    {
        var isAnon = post.IsAnonymous || post.Author.IsAnonDefault;
        var media = post.PostMediaItems
            .OrderBy(pm => pm.DisplayOrder)
            .Select(pm => new PostMediaResponseDto(
                pm.Id,
                pm.FileKey,
                r2Service.GetPublicUrl(pm.FileKey),
                pm.ContentType,
                pm.DisplayOrder,
                pm.FileSize,
                pm.OriginalFileName,
                pm.MediaType.ToString()))
            .ToList();

        return new PostResponseDto(
            post.Id,
            post.Title,
            post.Content,
            post.AuthorId,
            isAnon ? post.Author.AnonAlias : post.Author.Username,
            isAnon,
            post.SubjectId,
            post.Subject?.Name,
            media,
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
