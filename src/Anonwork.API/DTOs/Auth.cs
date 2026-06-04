using System.ComponentModel.DataAnnotations;

namespace Anonwork.API.DTOs;

public record RegisterRequestDto(
    [Required, MinLength(3), MaxLength(50)] string Username,
    [Required, EmailAddress] string Email,
    [Required, MinLength(8)] string Password,
    [MaxLength(80)] string? AnonAlias = null
);

public record VerifyEmailRequestDto(
    [Required, EmailAddress] string Email,
    [Required] string Token
);

public record LoginRequestDto(
    [Required, EmailAddress] string Email,
    [Required] string Password
);

public record GoogleLoginRequestDto(
    [Required] string IdToken,
    [MaxLength(80)] string? AnonAlias = null
);

public record RefreshRequestDto(
    [Required] string RefreshToken
);

public record LogoutRequestDto(
    [Required] string RefreshToken,
    [Required] string AccessToken
);

public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    Guid UserId,
    string AnonAlias
);