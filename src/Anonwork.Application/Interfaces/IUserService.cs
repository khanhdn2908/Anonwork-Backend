using Anonwork.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anonwork.Application.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllAsync();
    }
}
