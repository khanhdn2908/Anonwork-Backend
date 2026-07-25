using Anonwork.Application.Features.Search.DTOs.Response;

namespace Anonwork.Application.Features.Search;

/// <summary>
/// Use case for global search combining posts and users.
/// </summary>
public class SearchAllUseCase(
    SearchPostsUseCase searchPostsUseCase,
    SearchUsersUseCase searchUsersUseCase)
{
    public async Task<SearchAllResponseDto> ExecuteAsync(
        bool hasPostsPermission,
        bool hasUsersPermission,
        string? searchQuery,
        int limit = 5,
        Guid? currentUserId = null,
        CancellationToken ct = default)
    {
        if (limit < 1) limit = 5;
        if (limit > 20) limit = 20;

        var posts = await searchPostsUseCase.ExecuteAsync(
            hasPermission: hasPostsPermission,
            searchQuery: searchQuery,
            subjectId: null,
            tag: null,
            sortBy: "relevance",
            page: 1,
            pageSize: limit,
            currentUserId: currentUserId,
            ct: ct);

        var users = await searchUsersUseCase.ExecuteAsync(
            hasPermission: hasUsersPermission,
            searchQuery: searchQuery,
            page: 1,
            pageSize: limit,
            ct: ct);

        return new SearchAllResponseDto(
            Posts: posts,
            Users: users,
            Query: searchQuery ?? string.Empty
        );
    }
}
