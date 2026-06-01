namespace Anonwork.Application.Features.Auth.DTOs;

public record GoogleLoginRequest(
    string IdToken,
    string? AnonAlias = null
);
