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

        public async Task<IEnumerable<DepartmentKpi>> GetByDepartmentIdAsync(int departmentId, int cycleId)
        {
            return await _dbSet
                .Include(d => d.KpiMaster)
                .Where(d => d.DepartmentId == departmentId && d.CycleId == cycleId)
                .ToListAsync();
        }

        public override async Task<DepartmentKpi?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(d => d.KpiMaster)
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

        public async Task<IEnumerable<TeamKpi>> GetByTeamIdAsync(int teamId)
        {
            return await _dbSet
                .Include(t => t.DepartmentKpi)
                .ThenInclude(d => d.KpiMaster)
                .Where(t => t.TeamId == teamId)
                .ToListAsync();
        }

        public override async Task<TeamKpi?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(t => t.DepartmentKpi)
                .ThenInclude(d => d.KpiMaster)
                .Include(t => t.Team)
                .FirstOrDefaultAsync(t => t.TeamKpiId == id);
        }
    }

    public class EmployeeKpiRepository : BaseRepository<EmployeeKpi, int>, IEmployeeKpiRepository
    {
        public EmployeeKpiRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<EmployeeKpi>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _dbSet
                .Include(e => e.TeamKpi)
                .ThenInclude(t => t.DepartmentKpi)
                .ThenInclude(d => d.KpiMaster)
                .Where(e => e.EmployeeId == employeeId)
                .ToListAsync();
        }

        public override async Task<EmployeeKpi?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(e => e.TeamKpi)
                .ThenInclude(t => t.DepartmentKpi)
                .ThenInclude(d => d.KpiMaster)
                .Include(e => e.Employee)
                .FirstOrDefaultAsync(e => e.EmployeeKpiId == id);
        }
    }
}
