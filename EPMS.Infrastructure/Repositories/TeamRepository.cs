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
        using IDbConnection db = new SqlConnection(_connectionString);
        return await db.QueryAsync(Teams_GetByDepartment, new { DepartmentId = departmentId });
    }

    public async Task<Team?> GetByIdNoTrackingAsync(int id)
    {
        using IDbConnection db = new SqlConnection(_connectionString);
        string sql = "SELECT * FROM Teams WHERE TeamId = @Id AND IsDeleted = 0";
        return await db.QueryFirstOrDefaultAsync<Team>(sql, new { Id = id });
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Team>> GetDepartmentTeamsAsync(int departmentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(t => !t.IsDeleted && t.DepartmentId == departmentId)
            .Include(t => t.Manager)
            .Include(t => t.Department)
            .OrderBy(t => t.TeamName)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Team?> GetByIdInDepartmentAsync(int teamId, int departmentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(t => t.TeamId == teamId && !t.IsDeleted && t.DepartmentId == departmentId)
            .Include(t => t.Manager)
            .Include(t => t.Department)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
