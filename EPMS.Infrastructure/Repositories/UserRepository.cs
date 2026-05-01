using System;
using System.Threading.Tasks;
using System;
using System.Threading.Tasks;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace EPMS.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User, int>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username must be provided.", nameof(username));
        }

        return await _context.Users
            .AsNoTracking()
            .Include(u => u.Employee)
            .FirstOrDefaultAsync(u => u.Username == username);
    }
}
