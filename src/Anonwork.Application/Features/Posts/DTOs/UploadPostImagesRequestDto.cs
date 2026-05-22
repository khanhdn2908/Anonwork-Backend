using Microsoft.AspNetCore.Http;

namespace Anonwork.Application.Features.Posts.DTOs;

/// <summary>
/// DTO for uploading post images
/// </summary>
public class UploadPostImagesRequestDto
{
    /// <summary>
    /// Image files to upload
    /// </summary>
    public IFormFileCollection Images { get; set; } = null!;
}

/// <summary>
/// DTO for upload post images response
/// </summary>
public class UploadPostImagesResponseDto
{
    /// <summary>
    /// List of uploaded image URLs
    /// </summary>
    public List<string> ImageUrls { get; set; } = new();

    /// <summary>
    /// Number of successfully uploaded images
    /// </summary>
    public int UploadedCount { get; set; }

    /// <summary>
    /// Total number of images attempted to upload
    /// </summary>
    public int TotalCount { get; set; }
}
