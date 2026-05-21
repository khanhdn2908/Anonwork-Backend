namespace Anonwork.Application.UseCases.Auth;

public record AuthResult(
    string AccessToken,
    string RefreshToken,
    Guid UserId,
    string AnonAlias,
    string Role
);