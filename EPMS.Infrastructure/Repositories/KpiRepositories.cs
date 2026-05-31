using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPMS.Infrastructure.Repositories
{
    public class KpiRepository : BaseRepository<Kpi, int>, IKpiRepository
    {
        public KpiRepository(AppDbContext context) : base(context)
        {
        }
    }

    public class DepartmentKpiRepository : BaseRepository<DepartmentKpi, int>, IDepartmentKpiRepository
    {
        public DepartmentKpiRepository(AppDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<DepartmentKpi>> GetAllAsync()
        {
            return await _dbSet
                .Include(d => d.Kpi)
                .Include(d => d.Department)
                .Include(d => d.Cycle)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<DepartmentKpi>> GetByDepartmentIdAsync(int departmentId, int cycleId)
        {
            return await _dbSet
                .Include(d => d.Kpi)
                .Include(d => d.Department)
                .Include(d => d.Cycle)
                .Where(d => d.DepartmentId == departmentId && d.CycleId == cycleId)
                .AsNoTracking()
                .ToListAsync();
        }

        public override async Task<DepartmentKpi?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(d => d.Kpi)
                .Include(d => d.Department)
                .Include(d => d.Cycle)
                .FirstOrDefaultAsync(d => d.DeptKpiId == id);
        }
    }

    public class TeamKpiRepository : BaseRepository<TeamKpi, int>, ITeamKpiRepository
    {
        public TeamKpiRepository(AppDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<TeamKpi>> GetAllAsync()
        {
            return await _dbSet
                .Include(t => t.DepartmentKpi)
                .ThenInclude(d => d.Kpi)
                .Include(t => t.Team)
                .ToListAsync();
        }

        public async Task<IEnumerable<TeamKpi>> GetByTeamIdAsync(int teamId)
        {
            return await _dbSet
                .Include(t => t.DepartmentKpi)
                .ThenInclude(d => d.Kpi)
                .Include(t => t.Team)
                .Where(t => t.TeamId == teamId)
                .ToListAsync();
        }

        public override async Task<TeamKpi?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(t => t.DepartmentKpi)
                .ThenInclude(d => d.Kpi)
                .Include(t => t.Team)
                .FirstOrDefaultAsync(t => t.TeamKpiId == id);
        }
    }

    public class EmployeeKpiRepository : BaseRepository<EmployeeKpi, int>, IEmployeeKpiRepository
    {
        public EmployeeKpiRepository(AppDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<EmployeeKpi>> GetAllAsync()
        {
            return await _dbSet
                .Include(e => e.Employee)
                .Include(e => e.AppraisalCycle)
                .Include(e => e.Kpi)
                .Include(e => e.TeamKpi)
                .Include(e => e.PositionKpi)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeKpi>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _dbSet
                .Include(e => e.Employee)
                .Include(e => e.AppraisalCycle)
                .Include(e => e.Kpi)
                .Include(e => e.TeamKpi)
                .Include(e => e.PositionKpi)
                .Where(e => e.EmployeeId == employeeId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeKpi>> GetByTeamKpiIdAsync(int teamKpiId)
        {
            return await _dbSet
                .Include(e => e.Employee)
                .Include(e => e.AppraisalCycle)
                .Include(e => e.Kpi)
                .Include(e => e.TeamKpi)
                .Include(e => e.PositionKpi)
                .Where(e => e.TeamKpiId == teamKpiId)
                .AsNoTracking()
                .ToListAsync();
        }

        public override async Task<EmployeeKpi?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(e => e.Employee)
                .Include(e => e.AppraisalCycle)
                .Include(e => e.Kpi)
                .Include(e => e.TeamKpi)
                .Include(e => e.PositionKpi)
                .FirstOrDefaultAsync(e => e.EmployeeKpiId == id);
        }
    }
}
