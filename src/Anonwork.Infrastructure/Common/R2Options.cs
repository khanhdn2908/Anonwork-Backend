namespace Anonwork.Infrastructure.Common;

public class R2Options
{
    public const string SectionName = "R2";

    public string AccountId { get; set; } = string.Empty;

    public string AccessKeyId { get; set; } = string.Empty;

    public string SecretAccessKey { get; set; } = string.Empty;

    public string BucketName { get; set; } = string.Empty;

    public string PublicBaseUrl { get; set; } = string.Empty;

    public string Region { get; set; } = "auto";

    public string Endpoint { get; set; } = string.Empty;

    public string DefaultAvatarKey { get; set; } = "avatars/null.jpg";

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(AccountId))
            throw new InvalidOperationException("R2 AccountId is not configured.");
        if (string.IsNullOrWhiteSpace(AccessKeyId))
            throw new InvalidOperationException("R2 AccessKeyId is not configured.");
        if (string.IsNullOrWhiteSpace(SecretAccessKey))
            throw new InvalidOperationException("R2 SecretAccessKey is not configured.");
        if (string.IsNullOrWhiteSpace(BucketName))
            throw new InvalidOperationException("R2 BucketName is not configured.");
        if (string.IsNullOrWhiteSpace(Endpoint))
            throw new InvalidOperationException("R2 Endpoint is not configured.");
    }
}
