using EPMS.Domain.Entities;

namespace EPMS.Domain.Interfaces
{
    public interface IKpiRepository : IBaseRepository<Kpi, int>
    {
    }

    public interface IDepartmentKpiRepository : IBaseRepository<DepartmentKpi, int>
    {
        Task<IEnumerable<DepartmentKpi>> GetByDepartmentIdAsync(int departmentId, int cycleId);
    }

    public interface ITeamKpiRepository : IBaseRepository<TeamKpi, int>
    {
        Task<IEnumerable<TeamKpi>> GetByTeamIdAsync(int teamId);
    }

    public interface IEmployeeKpiRepository : IBaseRepository<EmployeeKpi, int>
    {
        Task<IEnumerable<EmployeeKpi>> GetByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<EmployeeKpi>> GetByTeamKpiIdAsync(int teamKpiId);
    }
}
