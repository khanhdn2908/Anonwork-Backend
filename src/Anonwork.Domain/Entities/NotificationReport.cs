using Anonwork.Domain.Common;
using Anonwork.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Anonwork.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid? ActorId { get; set; }
    public NotificationType Type { get; set; }
    public Guid? RefId { get; set; }
    public bool IsRead { get; set; } = false;
    public User User { get; set; } = null!;
    public User? Actor { get; set; }
}

public class Report : AuditableEntity
{
    public Guid ReporterId { get; set; }
    public Guid TargetId { get; set; }
    public ReportTargetType TargetType { get; set; }
    public string Reason { get; set; } = string.Empty;
    public ReportStatus Status { get; set; } = ReportStatus.pending;
    public User Reporter { get; set; } = null!;
}
