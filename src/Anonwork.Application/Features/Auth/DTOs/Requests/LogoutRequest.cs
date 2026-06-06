using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anonwork.Application.Features.Auth.DTOs.Requests
{
    public record LogoutRequest(Guid UserId, string RefreshToken, string AccessToken);
}
