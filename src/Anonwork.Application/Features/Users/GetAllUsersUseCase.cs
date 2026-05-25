using Anonwork.Application.Features.Users.DTOs;
using Anonwork.Application.Interfaces;

namespace Anonwork.Application.Features.Users;

public class GetAllUsersUseCase(IUserRepository userRepo)
{
    public async Task<UserListPaginatedResponseDto> ExecuteAsync(
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        // Validate pagination
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100; // Max 100 per page

        var (users, total) = await userRepo.GetAllAsync(page, pageSize, ct);

        var userDtos = users.Select(u => new UserListResponseDto(
            u.Id,
            u.Username,
            u.AvatarUrl,
            u.Bio,
            u.AnonAlias,
            u.Role,
            u.CreatedAt
        )).ToList();

        var totalPages = (int)Math.Ceiling(total / (double)pageSize);

        return new UserListPaginatedResponseDto(userDtos, total, page, pageSize, totalPages);
    }
}
