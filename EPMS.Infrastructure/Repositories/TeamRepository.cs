using System.Collections.Generic;
using System.Threading.Tasks;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Infrastructure.Contexts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static EPMS.Infrastructure.StoredProcedures;

namespace EPMS.Infrastructure.Repositories;

public class TeamRepository : BaseRepository<Team, int>, ITeamRepository
{
    private readonly ISqlRepository<Team, int> _sqlRepository;

    public TeamRepository(AppDbContext context, ISqlRepository<Team, int> sqlRepository) : base(context)
    {
        _sqlRepository = sqlRepository;
    }

    public override async Task<IEnumerable<Team>> GetAllAsync()
    {
        return await _dbSet.Include(t => t.Manager)
            .Include(t => t.Department)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<dynamic>> GetTeamsByDepartmentAsync(int departmentId)
    {
        // Using dynamic here because sp_GetTeamsByDepartment returns joined data (manager name, member count)
        // that doesn't map directly to the Team entity.
        var parameters = new[] { new SqlParameter("@DepartmentId", departmentId) };
        return await _sqlRepository.FromSqlAsync(Teams_GetByDepartment, parameters);
    }
}
