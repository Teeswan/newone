using System.Collections.Generic;
using System.Threading.Tasks;
using EPMS.Domain.Entities;
using Dapper;
using EPMS.Domain.Interfaces;
using EPMS.Infrastructure.Contexts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using static EPMS.Infrastructure.StoredProcedures;

namespace EPMS.Infrastructure.Repositories;

public class TeamRepository : BaseRepository<Team, int>, ITeamRepository
{
    private readonly ISqlRepository<Team, int> _sqlRepository;
    private readonly string _connectionString;

    public TeamRepository(AppDbContext context, ISqlRepository<Team, int> sqlRepository, IConfiguration configuration) : base(context)
    {
        _sqlRepository = sqlRepository;
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection is missing from configuration.");
    }

    public override async Task<IEnumerable<Team>> GetAllAsync()
    {
        return await _dbSet.Include(t => t.Manager)
            .Include(t => t.Department)
            .AsNoTracking()
            .ToListAsync();
    }

    public override async Task<Team?> GetByIdAsync(int id)
    {
        return await _dbSet.Include(t => t.Manager)
            .Include(t => t.Department)
            .FirstOrDefaultAsync(t => t.TeamId == id);
    }

    public override async Task<Team?> GetByIdFromDbAsync(int id)
    {
        return await _dbSet.Include(t => t.Manager)
            .Include(t => t.Department)
            .FirstOrDefaultAsync(t => t.TeamId == id);
    }

    public async Task<IEnumerable<dynamic>> GetTeamsByDepartmentAsync(int departmentId)
    {
        // Using dynamic here because sp_GetTeamsByDepartment returns joined data (manager name, member count)
        // that doesn't map directly to the Team entity.
        var parameters = new[] { new SqlParameter("@DepartmentId", departmentId) };
        return await _sqlRepository.FromSqlAsync(Teams_GetByDepartment, parameters);
    }

    public async Task<Team?> GetByIdNoTrackingAsync(int id)
    {
        using IDbConnection db = new SqlConnection(_connectionString);
        string sql = "SELECT * FROM Teams WHERE TeamId = @Id AND IsDeleted = 0";
        return await db.QueryFirstOrDefaultAsync<Team>(sql, new { Id = id });
    }
}
