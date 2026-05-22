namespace Anonwork.Infrastructure.Common;

/// <summary>
/// Cloudinary configuration options
/// </summary>
public class CloudinaryOptions
{
    public const string SectionName = "Cloudinary";

    public string CloudName { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    public string ApiSecret { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(CloudName))
            throw new InvalidOperationException("Cloudinary CloudName is not configured.");

        if (string.IsNullOrWhiteSpace(ApiKey))
            throw new InvalidOperationException("Cloudinary ApiKey is not configured.");

        if (string.IsNullOrWhiteSpace(ApiSecret))
            throw new InvalidOperationException("Cloudinary ApiSecret is not configured.");
    }
}
