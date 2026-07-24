using System.ComponentModel.DataAnnotations;

namespace Anonwork.Application.Features.PostRatings.DTOs.Requests;

public class RatePostRequestDto
{
    [Required]
    [Range(1, 5, ErrorMessage = "Stars must be between 1 and 5.")]
    public int Stars { get; set; }

    [MaxLength(500, ErrorMessage = "Review cannot exceed 500 characters.")]
    public string? Review { get; set; }
}
