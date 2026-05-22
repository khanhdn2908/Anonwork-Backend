using Anonwork.Application.Features.Posts.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Common.Exceptions;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Posts;

/// <summary>
/// Use case for getting posts by subject with pagination
/// </summary>
public class GetPostsBySubjectUseCase(IPostRepository postRepo)
{
    public async Task<PostListResponseDto> ExecuteAsync(
        Guid subjectId,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        // ── Validation ──────────────────────────────
        if (subjectId == Guid.Empty)
            throw new ArgumentException("Subject id is required.");

        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100; // Max 100 per page

        // ── Get posts ───────────────────────────────
        var (posts, total) = await postRepo.GetBySubjectAsync(subjectId, page, pageSize, ct);

        // ── Calculate pagination ────────────────────
        var totalPages = (int)Math.Ceiling((double)total / pageSize);

        // ── Return response ─────────────────────────
        var postDtos = posts.Select(MapToResponse).ToList();
        return new PostListResponseDto(postDtos, total, page, pageSize, totalPages);
    }

    private static PostResponseDto MapToResponse(Post post)
    {
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
            ImageUrls: post.PostImages
                .OrderBy(pi => pi.DisplayOrder)
                .Select(pi => pi.ImageUrl)
                .ToList(),
            Tags: post.PostTags
                .Select(pt => pt.Tag)
                .ToList(),
            Upvotes: post.Upvotes,
            CommentsCount: post.CommentsCount,
            ViewCount: post.ViewCount,
            Status: post.Status,
            CreatedAt: post.CreatedAt,
            UpdatedAt: post.UpdatedAt
        );
    }
}
