using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Application.Features.Admin;

public record ActivityLogResponseDto(
    Guid Id,
    Guid? UserId,
    string? UserUsername,
    string Action,
    string ActionCategory,
    string Description,
    string? TargetType,
    string? TargetId,
    string? IpAddress,
    string? UserAgent,
    string? DetailsJson,
    DateTime CreatedAt
);

public record ActivityLogListResponseDto(
    List<ActivityLogResponseDto> Logs,
    int Total,
    int Page,
    int PageSize,
    int TotalPages
);

public class GetActivityLogsUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<UserActivityLog> _logRepo = unitOfWork.GetRepository<UserActivityLog>();

    public async Task<ActivityLogListResponseDto> ExecuteAsync(
        int page = 1,
        int pageSize = 20,
        Guid? userId = null,
        string? actionCategory = null,
        string? search = null,
        CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var query = _logRepo.GetQueryableNoTracking()
            .Include(l => l.User)
            .AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(l => l.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(actionCategory))
        {
            query = query.Where(l => l.ActionCategory.ToLower() == actionCategory.Trim().ToLower());
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim().ToLower();
            query = query.Where(l =>
                l.Action.ToLower().Contains(keyword) ||
                l.Description.ToLower().Contains(keyword) ||
                (l.User != null && l.User.Username.ToLower().Contains(keyword)));
        }

        var total = await query.CountAsync(ct);

        var logs = await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var totalPages = (int)Math.Ceiling((double)total / pageSize);

        var dtos = logs.Select(l => new ActivityLogResponseDto(
            l.Id,
            l.UserId,
            l.User?.Username,
            l.Action,
            l.ActionCategory,
            l.Description,
            l.TargetType,
            l.TargetId,
            l.IpAddress,
            l.UserAgent,
            l.DetailsJson,
            l.CreatedAt
        )).ToList();

        return new ActivityLogListResponseDto(dtos, total, page, pageSize, totalPages);
    }
}
