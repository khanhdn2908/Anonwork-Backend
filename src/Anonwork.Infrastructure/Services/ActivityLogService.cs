using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Anonwork.Infrastructure.Services;

public class ActivityLogService : IActivityLogService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ActivityLogService> _logger;

    public ActivityLogService(IUnitOfWork unitOfWork, ILogger<ActivityLogService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task LogAsync(
        Guid? userId,
        string action,
        string category,
        string description,
        string? targetType = null,
        string? targetId = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? detailsJson = null,
        CancellationToken ct = default)
    {
        try
        {
            var log = new UserActivityLog
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Action = action,
                ActionCategory = category,
                Description = description,
                TargetType = targetType,
                TargetId = targetId,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                DetailsJson = detailsJson,
                CreatedAt = DateTime.UtcNow
            };

            var logRepo = _unitOfWork.GetRepository<UserActivityLog>();
            await logRepo.AddAsync(log, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write user activity log. Action: {Action}, UserId: {UserId}", action, userId);
        }
    }
}
