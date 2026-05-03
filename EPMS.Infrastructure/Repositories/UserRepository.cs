//using System;
//using System.Threading.Tasks;
//using System;
//using System.Threading.Tasks;
//using EPMS.Domain.Entities;
//using EPMS.Domain.Interfaces;
//using EPMS.Infrastructure.Contexts;
//using Microsoft.EntityFrameworkCore;

//namespace EPMS.Infrastructure.Repositories;

//public class UserRepository : BaseRepository<User, int>, IUserRepository
//{
//    public UserRepository(AppDbContext context) : base(context)
//    {
//    }

//    public async Task<User?> GetByUsernameAsync(string username)
//    {
//        if (string.IsNullOrWhiteSpace(username))
//        {
//            throw new ArgumentException("Username must be provided.", nameof(username));
//        }

//        return await _context.Users
//            .AsNoTracking()
//            .Include(u => u.Employee)
//            .FirstOrDefaultAsync(u => u.Username == username);
//    }
//}

using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace EPMS.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User, int>, IUserRepository
    {
        private readonly IMemoryCache _cache;
        private readonly string _cacheKey = "all_users_cache";

        public UserRepository(AppDbContext context, IMemoryCache cache) : base(context)
        {
            _cache = cache;
        }

        // ?? User ?????????? ????????????????? Cache ??????
        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            if (!_cache.TryGetValue(_cacheKey, out IEnumerable<User>? users))
            {
                users = await base.GetAllAsync();
                // User Data ? Employee ????? ??????????? ??????????? ? ???? ????????????
                _cache.Set(_cacheKey, users, TimeSpan.FromHours(1));
            }
            return users ?? new List<User>();
        }

        // ?? Login ???????? ????????? ??????????????? ????????????? Method
        public async Task<User?> GetByUsernameAsync(string username)
        {
            // ???????????????????? ??????? Cache Key ?????? ??????????????
            string userCacheKey = $"user_{username}";

            if (!_cache.TryGetValue(userCacheKey, out User? user))
            {
                user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user != null)
                {
                    _cache.Set(userCacheKey, user, TimeSpan.FromMinutes(30));
                }
            }
            return user;
        }

        // ?? Data ?????????????????? Cache ????????
        public override async Task<User> CreateAsync(User entity)
        {
            var result = await base.CreateAsync(entity);
            _cache.Remove(_cacheKey);
            return result;
        }

        public override async Task<User?> UpdateAsync(User entity)
        {
            var result = await base.UpdateAsync(entity);
            _cache.Remove(_cacheKey);
            _cache.Remove($"user_{entity.Username}"); // ??????????????? Cache ????? ????????
            return result;
        }
    }
}
