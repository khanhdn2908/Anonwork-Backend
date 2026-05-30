using Anonwork.Application.Features.Users.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Users;

public class GetAllUsersUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<User> _userRepo = unitOfWork.GetRepository<User>();

    public async Task<UserListPaginatedResponseDto> ExecuteAsync(
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var users = (await _userRepo.GetAllAsync(ct)).OrderByDescending(u => u.CreatedAt);
        var total = await _userRepo.CountAsync(ct);

        var pagedUsers = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var userDtos = pagedUsers.Select(u => new UserListResponseDto(
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
