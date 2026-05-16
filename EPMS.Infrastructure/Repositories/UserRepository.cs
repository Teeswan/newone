

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
            return result;
        }
    }
}
