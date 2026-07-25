using Anonwork.Application.Features.Posts.DTOs.Response;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Posts.Helpers;

public static class PostVoteProjectionHelper
{
    public static PostResponseDto MapToResponse(Post post, bool isUpvotedByMe, IR2Service r2Service, int? myStars = null)
    {
        var author = post.Author;
        var isAnon = post.IsAnonymous || author?.IsAnonDefault == true;
        var media = (post.PostMediaItems ?? new List<PostMedia>())
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

        var authorImageUrl = isAnon
            ? (!string.IsNullOrWhiteSpace(author?.AnonImage?.FileKey)
                ? r2Service.GetPublicUrl(author!.AnonImage!.FileKey)
                : r2Service.GetPublicUrl("avatars/null.jpg"))
            : (string.IsNullOrWhiteSpace(author?.AvatarKey)
                ? r2Service.GetPublicUrl("avatars/null.jpg")
                : r2Service.GetPublicUrl(author!.AvatarKey));

        return new PostResponseDto(
            post.Id,
            post.Title,
            post.Content,
            post.AuthorId,
            isAnon ? author?.AnonAlias : author?.Username,
            isAnon,
            authorImageUrl,
            post.SubjectId,
            post.Subject?.Name,
            media,
            (post.PostTags ?? new List<PostTag>()).Select(pt => pt.Tag).ToList(),
            post.Upvotes,
            post.CommentsCount,
            post.ViewCount,
            post.AverageRating,
            post.RatingsCount,
            post.QualityScore,
            myStars,
            post.Status.ToString(),
            post.CreatedAt,
            post.UpdatedAt,
            isUpvotedByMe
        );
    }
}
