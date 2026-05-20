using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;
using Anonwork.Domain.Repositories;
using Anonwork.Infrastructure.Model;
using Anonwork.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anonwork.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        //private readonly Supabase.Client _supabase;

        //public UserRepository(Supabase.Client supabase)
        //{
        //    _supabase = supabase;
        //}

        //public async Task<List<User>> GetAllAsync()
        //{
        //    var response = await _supabase.From<UserTable>().Get();

        //    return response.Models.Select(ut => new User
        //    {
        //        Id = ut.Id,
        //        Username = ut.Username,
        //        AvatarUrl = ut.AvatarUrl,
        //        Bio = ut.Bio,
        //        AnonAlias = ut.AnonAlias,
        //        IsAnonDefault = ut.IsAnonDefault,
        //        Role = Enum.TryParse(ut.Role, true, out UserRole role) ? role : UserRole.Student,
        //        CreatedAt = ut.CreatedAt,
        //        UpdatedAt = ut.UpdatedAt
        //    }).ToList();
        //}

        private readonly AppDbContext _appDbContext;

        public UserRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<User>> GetAllAsync()
        {
            var response = await _appDbContext.Users.ToListAsync();

            return response;
        }
    }
}
