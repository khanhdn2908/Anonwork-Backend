using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anonwork.Application.Features.Payments.DTOs
{
    public sealed class WebhookResult
    {
        public bool Success { get; private init; }
        public string? ErrorMessage { get; private init; }

        private WebhookResult() { }

        public static WebhookResult Ok(string v) => new() { Success = true };
        public static WebhookResult Fail(string message) => new() { Success = false, ErrorMessage = message };

        internal static WebhookResult Ok()
        {
            throw new NotImplementedException();
        }
    }
}
