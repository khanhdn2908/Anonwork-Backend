namespace Anonwork.Application.Features.Payments.DTOs.Responses;

public record OrderResponse(
    Guid Id,
    string OrderCode,
    string TransferContent,
    string QrUrl,
    decimal Amount,
    string Status,
    string? AccountName,
    string? BankName,
    string? BankAccount
);