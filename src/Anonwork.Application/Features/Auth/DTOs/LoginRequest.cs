using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anonwork.Application.Features.Auth.DTOs
{
    public record LoginRequest(
        string Email,
        string Password
    );
}
