using Anonwork.Application.Features.Posts.DTOs;
using Anonwork.Application.Interfaces;
using Post = Anonwork.Domain.Entities.Post;

namespace Anonwork.Application.Features.Posts;

/// <summary>
/// Use case for searching posts using full-text search
/// </summary>
public class SearchPostsUseCase(IPostRepository postRepo)
{
    public async Task<PostListResponseDto> ExecuteAsync(
        string query,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        // ── Validation ──────────────────────────────
        if (string.IsNullOrWhiteSpace(query))
            throw new ArgumentException("Search query is required.");

        if (query.Length < 2)
            throw new ArgumentException("Search query must be at least 2 characters.");

        if (page < 1)
            throw new ArgumentException("Page must be greater than 0.");

        if (pageSize < 1 || pageSize > 100)
            throw new ArgumentException("Page size must be between 1 and 100.");

        // ── Search posts ────────────────────────────
        var (posts, total) = await postRepo.SearchAsync(query, page, pageSize, ct);

        // ── Calculate total pages ───────────────────
        var totalPages = (total + pageSize - 1) / pageSize;

        // ── Map to response ─────────────────────────
        var postDtos = posts.Select(MapToResponse).ToList();

        return new PostListResponseDto(
            Posts: postDtos,
            Total: total,
            Page: page,
            PageSize: pageSize,
            TotalPages: totalPages
        );
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
