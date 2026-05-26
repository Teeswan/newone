using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Infrastructure.Contexts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static EPMS.Infrastructure.StoredProcedures;

namespace EPMS.Infrastructure.Repositories;

public class PerformanceEvaluationRepository : BaseRepository<PerformanceEvaluation, int>, IPerformanceEvaluationRepository
{
    private readonly ISqlRepository<PerformanceEvaluation, int> _sqlRepository;

    public PerformanceEvaluationRepository(AppDbContext context, ISqlRepository<PerformanceEvaluation, int> sqlRepository) : base(context)
    {
        _sqlRepository = sqlRepository ?? throw new ArgumentNullException(nameof(sqlRepository));
    }

    public override async Task<IEnumerable<PerformanceEvaluation>> GetAllAsync()
    {
        return await _dbSet
            .Include(pe => pe.Employee)
            .Include(pe => pe.Cycle)
            .Include(pe => pe.Form)
            .AsNoTracking()
            .ToListAsync();
    }

    public override async Task<PerformanceEvaluation?> GetByIdAsync(int evalId)
    {
        return await _dbSet
            .Include(pe => pe.Employee)
            .Include(pe => pe.Cycle)
            .Include(pe => pe.Form)
            .Include(pe => pe.AppraisalResponses)
                .ThenInclude(ar => ar.Question)
            .Include(pe => pe.AppraisalResponses)
                .ThenInclude(ar => ar.Respondent)
            .FirstOrDefaultAsync(pe => pe.EvalId == evalId);
    }

    public override async Task<PerformanceEvaluation> CreateAsync(PerformanceEvaluation entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var parameters = new object[]
        {
            new SqlParameter("@EmployeeID", ToDbValue(entity.EmployeeId)),
            new SqlParameter("@CycleID", ToDbValue(entity.CycleId)),
            new SqlParameter("@FormID", entity.FormId),
            new SqlParameter("@Status", (int)entity.Status),
            new SqlParameter("@SelfRating", ToDbValue(entity.SelfRating)),
            new SqlParameter("@ManagerRating", ToDbValue(entity.ManagerRating)),
            new SqlParameter("@SelfComments", ToDbValue(entity.SelfComments)),
            new SqlParameter("@ManagerComments", ToDbValue(entity.ManagerComments)),
            new SqlParameter("@FinalRatingScore", ToDbValue(entity.FinalRatingScore)),
            new SqlParameter("@IsFinalized", ToDbValue(entity.IsFinalized)),
            new SqlParameter("@FinalizedAt", ToDbValue(entity.FinalizedAt))
        };

        var result = await _sqlRepository.FromSqlFirstOrDefaultAsync(PerformanceEvaluations_Create, parameters);
        return result ?? throw new InvalidOperationException("Failed to create performance evaluation.");
    }

    public override async Task<PerformanceEvaluation?> UpdateAsync(PerformanceEvaluation entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var parameters = new object[]
        {
            new SqlParameter("@EvalID", entity.EvalId),
            new SqlParameter("@EmployeeID", ToDbValue(entity.EmployeeId)),
            new SqlParameter("@CycleID", ToDbValue(entity.CycleId)),
            new SqlParameter("@FormID", entity.FormId),
            new SqlParameter("@Status", (int)entity.Status),
            new SqlParameter("@SelfRating", ToDbValue(entity.SelfRating)),
            new SqlParameter("@ManagerRating", ToDbValue(entity.ManagerRating)),
            new SqlParameter("@SelfComments", ToDbValue(entity.SelfComments)),
            new SqlParameter("@ManagerComments", ToDbValue(entity.ManagerComments)),
            new SqlParameter("@FinalRatingScore", ToDbValue(entity.FinalRatingScore)),
            new SqlParameter("@IsFinalized", ToDbValue(entity.IsFinalized)),
            new SqlParameter("@FinalizedAt", ToDbValue(entity.FinalizedAt))
        };

        return await _sqlRepository.FromSqlFirstOrDefaultAsync(PerformanceEvaluations_Update, parameters);
    }

    public override async Task<bool> DeleteAsync(int evalId)
    {
        var rowsAffected = await _sqlRepository.ExecuteSqlAsync(PerformanceEvaluations_Delete, new SqlParameter("@EvalID", evalId));
        return rowsAffected > 0;
    }

    public async Task<IEnumerable<PerformanceEvaluation>> GetByEmployeeIdAsync(int employeeId)
    {
        return await _dbSet
            .Include(pe => pe.Employee)
            .Include(pe => pe.Cycle)
            .Include(pe => pe.Form)
            .Where(pe => pe.EmployeeId == employeeId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<PerformanceEvaluation>> GetByCycleIdAsync(int cycleId)
    {
        return await _dbSet
            .Include(pe => pe.Employee)
            .Include(pe => pe.Cycle)
            .Include(pe => pe.Form)
            .Where(pe => pe.CycleId == cycleId)
            .AsNoTracking()
            .ToListAsync();
    }

    private static object ToDbValue<T>(T? value)
        => value is null ? DBNull.Value : value!;
}
