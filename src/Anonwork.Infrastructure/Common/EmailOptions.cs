namespace Anonwork.Infrastructure.Common;

public class EmailOptions
{
    public const string SectionName = "Email";

    public string ResendApiKey { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
    public string FromName { get; set; } = "Anonwork";
}
