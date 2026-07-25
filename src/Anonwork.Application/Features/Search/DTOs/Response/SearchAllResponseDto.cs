using Anonwork.Application.Features.Posts.DTOs.Response;
using Anonwork.Application.Features.Users.DTOs.Responses;

namespace Anonwork.Application.Features.Search.DTOs.Response;

public record SearchAllResponseDto(
    PostListResponseDto Posts,
    UserListPaginatedResponseDto Users,
    string Query
);
