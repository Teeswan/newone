
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Infrastructure.Contexts;
using Microsoft.Extensions.Caching.Memory;

namespace EPMS.Infrastructure.Repositories
{
    public class RoleRepository : BaseRepository<Role, int>, IRoleRepository
    {
        private readonly IMemoryCache _cache;
        private readonly string _cacheKey = "all_roles_cache";

        public RoleRepository(AppDbContext context, IMemoryCache cache) : base(context)
        {
            _cache = cache;
        }

        public override async Task<IEnumerable<Role>> GetAllAsync()
        {
            if (!_cache.TryGetValue(_cacheKey, out IEnumerable<Role>? roles))
            {
                roles = await base.GetAllAsync();
                _cache.Set(_cacheKey, roles, TimeSpan.FromHours(1));
            }
            return roles ?? new List<Role>();
        }

      
        public override async Task<Role> CreateAsync(Role entity)
        {
            var result = await base.CreateAsync(entity);
            _cache.Remove(_cacheKey); 
            return result;
        }

       
        public override async Task<Role?> UpdateAsync(Role entity)
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
    }
}