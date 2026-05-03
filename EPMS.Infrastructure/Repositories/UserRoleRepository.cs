
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Infrastructure.Contexts;
using Microsoft.Extensions.Caching.Memory;

namespace EPMS.Infrastructure.Repositories
{
    public class UserRoleRepository : BaseRepository<UserRole, int>, IUserRoleRepository
    {
        private readonly IMemoryCache _cache;
        private readonly string _cacheKey = "all_user_roles_cache";

        public UserRoleRepository(AppDbContext context, IMemoryCache cache) : base(context)
        {
            _cache = cache;
        }

       
        public override async Task<IEnumerable<UserRole>> GetAllAsync()
        {
            if (!_cache.TryGetValue(_cacheKey, out IEnumerable<UserRole>? userRoles))
            {
                userRoles = await base.GetAllAsync();
                _cache.Set(_cacheKey, userRoles, TimeSpan.FromHours(1));
            }
            return userRoles ?? new List<UserRole>();
        }

        public override async Task<UserRole> CreateAsync(UserRole entity)
        {
            var result = await base.CreateAsync(entity);
            _cache.Remove(_cacheKey);
            return result;
        }

     
        public override async Task<UserRole?> UpdateAsync(UserRole entity)
        {
            var result = await base.UpdateAsync(entity);
            _cache.Remove(_cacheKey);
            return result;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var result = await base.DeleteAsync(id);
            if (result)
            {
                _cache.Remove(_cacheKey);
            }
            return result;
        }

        public async Task<IEnumerable<UserRole>> GetAllUserRolesAsync()
        {
        
            return await GetAllAsync();
        }
    }
}