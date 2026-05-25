using System.ComponentModel.DataAnnotations;

namespace Anonwork.Application.Features.Users.DTOs;

public record UpdateUserRequestDto(
    [MaxLength(50)] string? Username,
    [MaxLength(500)] string? Bio,
    [Url] string? AvatarUrl,
    bool? IsAnonDefault
);
