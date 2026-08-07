namespace Anonwork.Infrastructure.Common;

public class SepayOptions
{
    public const string SectionName = "Sepay";

    public string BankAccount { get; set; } = string.Empty;

    public string BankCode { get; set; } = string.Empty;

    public string AccountName { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    public string ApiSecret { get; set; } = string.Empty;
}