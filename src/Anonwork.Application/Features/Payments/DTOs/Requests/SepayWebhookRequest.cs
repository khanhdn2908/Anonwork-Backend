using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Anonwork.Application.Features.Payments.DTOs.Requests;

public class SepayWebhookRequest
{
    public int Id { get; set; }

    public string Gateway { get; set; } = default!;

    [JsonPropertyName("transactionDate")]
    [JsonConverter(typeof(SepayDateTimeConverter))]
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

    public class SepayDateTimeConverter : JsonConverter<DateTime>
    {
        private const string Format = "yyyy-MM-dd HH:mm:ss";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var str = reader.GetString();
            return DateTime.ParseExact(str!, Format, CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString(Format));
    }
}