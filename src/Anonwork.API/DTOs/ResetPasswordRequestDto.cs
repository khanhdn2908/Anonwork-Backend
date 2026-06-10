namespace Anonwork.API.DTOs;

public sealed record ResetPasswordRequestDto(
    string Email,
    string Token,
    string NewPassword
);
