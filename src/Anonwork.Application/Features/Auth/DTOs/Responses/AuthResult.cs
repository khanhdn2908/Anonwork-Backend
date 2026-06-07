namespace Anonwork.Application.Features.Auth.DTOs.Responses;

public record AuthResult(
    string AccessToken,
    string RefreshToken,
    Guid UserId,
    string AnonAlias
);