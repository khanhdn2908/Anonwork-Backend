using Anonwork.Application.Features.Posts.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Posts;

/// <summary>
/// Use case for getting posts with pagination and search
/// </summary>
public class GetPostsUseCase(IPostRepository postRepo)
{
    public async Task<PostListResponseDto> ExecuteAsync(
        int page = 1,
        int pageSize = 10,
        string? searchQuery = null,
        CancellationToken ct = default)
    {
        // ── Validation ──────────────────────────────
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100; // Max 100 per page

        // ── Get posts ───────────────────────────────
        var (posts, total) = string.IsNullOrWhiteSpace(searchQuery)
            ? await postRepo.GetAllAsync(page, pageSize, ct)
            : await postRepo.SearchAsync(searchQuery, page, pageSize, ct);

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
