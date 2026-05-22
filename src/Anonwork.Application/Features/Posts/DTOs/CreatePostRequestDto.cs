using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Anonwork.Application.Features.Posts.DTOs;

/// <summary>
/// DTO for creating a new post
/// </summary>
public class CreatePostRequestDto
{
    [Required, MinLength(5), MaxLength(255)]
    public string Title { get; set; } = null!;

    [Required, MinLength(10)]
    public string Content { get; set; } = null!;

    [Required]
    public Guid SubjectId { get; set; }

    [MaxLength(5)]
    public List<string>? Tags { get; set; }

    public bool IsAnonymous { get; set; }

    /// <summary>
    /// Image files to upload (max 5 files, max 5MB each)
    /// </summary>
    public IFormFileCollection? Images { get; set; }
}
