using Anonwork.Domain.Enums;
using System;
using System.Collections.Generic;

namespace Anonwork.Domain.Entities;

public partial class Report
{
    public Guid Id { get; set; }

    public Guid ReporterId { get; set; }

    public Guid TargetId { get; set; }

    public string TargetType { get; set; } = null!;

    public string Reason { get; set; } = null!;

    public ReportStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual User Reporter { get; set; } = null!;
}
