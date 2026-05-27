using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anonwork.Application.Features.Payments.DTOs
{
    public record SepayQrResponse(
        string QrUrl,
        string TransferContent
    );
}
