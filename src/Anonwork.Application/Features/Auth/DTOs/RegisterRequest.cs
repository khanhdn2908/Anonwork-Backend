namespace Anonwork.Application.Features.Auth.DTOs;

public record RegisterRequest(
    string Username,
    string Email,
    string Password,
    string? AnonAlias = null
);