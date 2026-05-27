namespace Anonwork.Application.Features.Payments.DTOs;

public class SepayWebhookRequest
{
    public int Id { get; set; }

    public string Gateway { get; set; } = default!;

    public DateTime TransactionDate { get; set; }

    public string AccountNumber { get; set; } = default!;

    public string? SubAccount { get; set; }

    public string? Code { get; set; }

    public string Content { get; set; } = default!;

    public string TransferType { get; set; } = default!;

    public string? Description { get; set; }

    public long TransferAmount { get; set; }

    public long Accumulated { get; set; }

    public string ReferenceCode { get; set; } = default!;
}