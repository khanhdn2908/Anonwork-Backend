namespace Anonwork.Application.Features.Auth.DTOs.Requests;

public record GoogleLoginRequest(
    string IdToken,
    string? AnonAlias = null
);
