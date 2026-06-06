namespace Anonwork.Application.Features.Auth.DTOs.Requests;

public record RegisterRequest(
    string Username,
    string Email,
    string Password,
    string? AnonAlias = null
);