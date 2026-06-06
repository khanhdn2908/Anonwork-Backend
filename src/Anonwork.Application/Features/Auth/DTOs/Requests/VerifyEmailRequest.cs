namespace Anonwork.Application.Features.Auth.DTOs.Requests;

public record VerifyEmailRequest(
    string Email,
    string Token
);