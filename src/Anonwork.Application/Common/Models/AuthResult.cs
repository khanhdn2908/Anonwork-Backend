namespace Anonwork.Application.Common.Model;

public record AuthResult(
    string AccessToken,
    string RefreshToken,
    Guid UserId,
    string AnonAlias
);