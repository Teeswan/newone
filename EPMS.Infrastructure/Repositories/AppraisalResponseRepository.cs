using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using static EPMS.Infrastructure.StoredProcedures;

namespace EPMS.Infrastructure.Repositories;

/// <summary>
/// Repository for appraisal responses, implemented with stored procedure support.
/// </summary>
public class AppraisalResponseRepository : BaseRepository<AppraisalResponse, long>, IAppraisalResponseRepository
{
    private readonly ISqlRepository<AppraisalResponse, long> _sqlRepository;

    public AppraisalResponseRepository(AppDbContext context, ISqlRepository<AppraisalResponse, long> sqlRepository) : base(context)
    {
        _sqlRepository = sqlRepository ?? throw new ArgumentNullException(nameof(sqlRepository));
    }

    public override async Task<IEnumerable<AppraisalResponse>> GetAllAsync()
    {
        return await _dbSet
            .Include(ar => ar.Question)
                .ThenInclude(q => q.FormQuestions)
            .Include(ar => ar.Respondent)
            .Include(ar => ar.Eval)
            .AsNoTracking()
            .ToListAsync();
    }

    public override async Task<AppraisalResponse?> GetByIdAsync(long responseId)
    {
        return await _dbSet
            .Include(ar => ar.Question)
                .ThenInclude(q => q.FormQuestions)
            .Include(ar => ar.Respondent)
            .Include(ar => ar.Eval)
            .FirstOrDefaultAsync(ar => ar.ResponseId == responseId);
    }

    public override async Task<AppraisalResponse> CreateAsync(AppraisalResponse entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var parameters = new object[]
        {
            new SqlParameter("@EvalID", ToDbValue(entity.EvalId)),
            new SqlParameter("@QuestionID", ToDbValue(entity.QuestionId)),
            new SqlParameter("@RespondentID", ToDbValue(entity.RespondentId)),
            new SqlParameter("@RespondentEmployeeID", ToDbValue(entity.RespondentEmployeeId)),
            new SqlParameter("@RespondentRole", ToDbValue(entity.RespondentRole)),
            new SqlParameter("@AnswerText", ToDbValue(entity.AnswerText)),
            new SqlParameter("@RatingValue", ToDbValue(entity.RatingValue)),
            new SqlParameter("@IsAnonymous", ToDbValue(entity.IsAnonymous))
        };

        var result = await _sqlRepository.FromSqlFirstOrDefaultAsync(AppraisalResponses_Create, parameters);
        return result ?? throw new InvalidOperationException("Failed to create appraisal response.");
    }

    public override Task<AppraisalResponse?> UpdateAsync(AppraisalResponse entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var parameters = new object[]
        {
            new SqlParameter("@ResponseID", entity.ResponseId),
            new SqlParameter("@EvalID", ToDbValue(entity.EvalId)),
            new SqlParameter("@QuestionID", ToDbValue(entity.QuestionId)),
            new SqlParameter("@RespondentID", ToDbValue(entity.RespondentId)),
            new SqlParameter("@RespondentEmployeeID", ToDbValue(entity.RespondentEmployeeId)),
            new SqlParameter("@RespondentRole", ToDbValue(entity.RespondentRole)),
            new SqlParameter("@AnswerText", ToDbValue(entity.AnswerText)),
            new SqlParameter("@RatingValue", ToDbValue(entity.RatingValue)),
            new SqlParameter("@IsAnonymous", ToDbValue(entity.IsAnonymous))
        };

        return _sqlRepository.FromSqlFirstOrDefaultAsync(AppraisalResponses_Update, parameters);
    }

    public override async Task<bool> DeleteAsync(long responseId)
    {
        var rowsAffected = await _sqlRepository.ExecuteSqlAsync(AppraisalResponses_Delete, new SqlParameter("@ResponseID", responseId));
        return rowsAffected > 0;
    }

    public async Task<IEnumerable<AppraisalResponse>> GetByEvalIdAsync(int evalId)
    {
        if (evalId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(evalId));
        }

        return await _dbSet
            .Include(ar => ar.Question)
                .ThenInclude(q => q.FormQuestions)
            .Include(ar => ar.Respondent)
            .Include(ar => ar.Eval)
            .Where(ar => ar.EvalId == evalId)
            .AsNoTracking()
            .ToListAsync();
    }

    private static object ToDbValue<T>(T? value)
        => value is null ? DBNull.Value : value!;
}
