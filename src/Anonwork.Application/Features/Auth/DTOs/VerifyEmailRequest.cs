namespace Anonwork.Application.Features.Auth.DTOs;

public record VerifyEmailRequest(
    string Email,
    string Token
);