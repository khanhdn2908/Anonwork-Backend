using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Anonwork.Application.Features.Posts.DTOs;

/// <summary>
/// DTO for updating a post
/// </summary>
public class UpdatePostRequestDto
{
    [MinLength(5), MaxLength(255)]
    public string? Title { get; set; }

    [MinLength(10)]
    public string? Content { get; set; }

    [MaxLength(5)]
    public List<string>? Tags { get; set; }

    /// <summary>
    /// New image files to add (max 5 total)
    /// </summary>
    public IFormFileCollection? NewImages { get; set; }

    /// <summary>
    /// Image URLs to remove
    /// </summary>
    public List<string>? RemoveImageUrls { get; set; }
}
