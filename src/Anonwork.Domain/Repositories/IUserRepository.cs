using Anonwork.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anonwork.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
    }
}
