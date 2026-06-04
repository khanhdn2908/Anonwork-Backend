namespace Anonwork.Application.Common;

public class MaintenanceOptions
{
    public const string SectionName = "Maintenance";

    public string CleanupSecret { get; set; } = string.Empty;
    public int EmailVerificationTokenRetentionDays { get; set; } = 7;
}
