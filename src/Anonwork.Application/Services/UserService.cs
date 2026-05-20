using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Anonwork.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IDistributedCache _cache;

        private const string AllUsersCacheKey = "users:all";
        public UserService(IUserRepository repo, IDistributedCache cache)
        {
            _repo = repo;
            _cache = cache;
        }
        public async Task<List<User>> GetAllAsync()
        {
            var cached = await _cache.GetStringAsync(AllUsersCacheKey);
            if (cached is not null)
                return JsonSerializer.Deserialize<List<User>>(cached)!;

            // 2. Cache miss → query DB
            var users = await _repo.GetAllAsync();

            // 3. Lưu vào cache 5 phút
            await _cache.SetStringAsync(
                AllUsersCacheKey,
                JsonSerializer.Serialize(users),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

            return users;
        }
    }
}
