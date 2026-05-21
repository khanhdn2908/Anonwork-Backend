using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anonwork.Application.UseCases.Auth
{
    public record RegisterRequest(
     string Username,
     string Email,
     string Password,
     string? AnonAlias = null
    );
}
