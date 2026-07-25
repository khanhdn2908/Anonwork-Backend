namespace Anonwork.Application.Interfaces;

public interface IActivityLogService
{
    Task LogAsync(
        Guid? userId,
        string action,
        string category,
        string description,
        string? targetType = null,
        string? targetId = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? detailsJson = null,
        CancellationToken ct = default);
}
