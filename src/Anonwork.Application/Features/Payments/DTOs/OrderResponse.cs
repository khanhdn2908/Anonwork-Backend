namespace Anonwork.Application.Features.Payments.DTOs;

public record OrderResponse(
    Guid Id,
    string OrderCode,
    string TransferContent,
    string QrUrl,
    decimal Amount,
    string Status
);