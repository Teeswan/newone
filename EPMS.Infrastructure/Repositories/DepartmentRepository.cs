using System.Collections.Generic;
using System.Threading.Tasks;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Infrastructure.Contexts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static EPMS.Infrastructure.StoredProcedures;

namespace EPMS.Infrastructure.Repositories;

public class DepartmentRepository : BaseRepository<Department, int>, IDepartmentRepository
{
    private readonly ISqlRepository<Department, int> _sqlRepository;

    public DepartmentRepository(AppDbContext context, ISqlRepository<Department, int> sqlRepository) : base(context)
    {
        _sqlRepository = sqlRepository ?? throw new ArgumentNullException(nameof(sqlRepository));
    }

    public async Task<IEnumerable<Department>> GetDepartmentTreeAsync()
    {
        return await _sqlRepository.FromSqlAsync(Departments_GetTree);
    }

    public override async Task<IEnumerable<Department>> GetAllAsync()
    {
        return await _dbSet.Where(d => d.IsDeleted == false)
            .Include(d => d.ParentDepartment)
            .AsNoTracking()
            .ToListAsync();
    }

    public override async Task<Department?> GetByIdAsync(int id)
    {
        return await _dbSet.Include(d => d.ParentDepartment)
            .FirstOrDefaultAsync(d => d.DepartmentId == id);
    }

    public override async Task<Department?> GetByIdFromDbAsync(int id)
    {
        return await _dbSet.Include(d => d.ParentDepartment)
            .FirstOrDefaultAsync(d => d.DepartmentId == id);
    }
}
