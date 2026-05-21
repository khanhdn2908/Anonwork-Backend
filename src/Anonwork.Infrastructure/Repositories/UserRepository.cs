using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
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

        // ──────────────────────────────────────────
        // READ
        // ──────────────────────────────────────────

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _appDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id, ct);

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
            => await _appDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email.ToLower().Trim(), ct);

        public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
            => await _appDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username.ToLower().Trim(), ct);

        // ──────────────────────────────────────────
        // EXISTS (dùng cho validation, chỉ SELECT 1)
        // ──────────────────────────────────────────

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
            => await _appDbContext.Users
                .AnyAsync(u => u.Email == email.ToLower().Trim(), ct);

        public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct = default)
            => await _appDbContext.Users
                .AnyAsync(u => u.Username == username.ToLower().Trim(), ct);

        public async Task<bool> ExistsByAnonAliasAsync(string alias, CancellationToken ct = default)
            => await _appDbContext.Users
                .AnyAsync(u => u.AnonAlias == alias, ct);

        // ──────────────────────────────────────────
        // WRITE
        // ──────────────────────────────────────────

        public async Task<User> CreateAsync(User user, CancellationToken ct = default)
        {
            _appDbContext.Users.Add(user);
            await _appDbContext.SaveChangesAsync(ct);
            return user;
        }

        public async Task UpdateAsync(User user, CancellationToken ct = default)
        {
            _appDbContext.Users.Update(user);
            await _appDbContext.SaveChangesAsync(ct);
        }
    }
}
