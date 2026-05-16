using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Infrastructure.Contexts;
using Microsoft.Data.SqlClient;
using static EPMS.Infrastructure.StoredProcedures;

namespace EPMS.Infrastructure.Repositories;

/// <summary>
/// Repository for appraisal questions, implemented with stored procedure support.
/// </summary>
public class AppraisalQuestionRepository : BaseRepository<AppraisalQuestion, int>, IAppraisalQuestionRepository
{
    private readonly ISqlRepository<AppraisalQuestion, int> _sqlRepository;

    public AppraisalQuestionRepository(AppDbContext context, ISqlRepository<AppraisalQuestion, int> sqlRepository) : base(context)
    {
        _sqlRepository = sqlRepository ?? throw new ArgumentNullException(nameof(sqlRepository));
    }

    public override Task<IEnumerable<AppraisalQuestion>> GetAllAsync()
    {
        return _sqlRepository.FromSqlAsync(AppraisalQuestions_GetAll);
    }

    public override Task<AppraisalQuestion?> GetByIdAsync(int questionId)
    {
        return _sqlRepository.FromSqlFirstOrDefaultAsync(AppraisalQuestions_GetById, new SqlParameter("@QuestionID", questionId));
    }

    public override async Task<AppraisalQuestion> CreateAsync(AppraisalQuestion entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var parameters = new object[]
        {
            new SqlParameter("@QuestionText", entity.QuestionText),
            new SqlParameter("@Category", ToDbValue(entity.Category)),
            new SqlParameter("@IsRequired", ToDbValue(entity.IsRequired))
        };

        var result = await _sqlRepository.FromSqlFirstOrDefaultAsync(AppraisalQuestions_Create, parameters);
        return result ?? throw new InvalidOperationException("Failed to create appraisal question.");
    }

    public override Task<AppraisalQuestion?> UpdateAsync(AppraisalQuestion entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var parameters = new object[]
        {
            new SqlParameter("@QuestionID", entity.QuestionId),
            new SqlParameter("@QuestionText", entity.QuestionText),
            new SqlParameter("@Category", ToDbValue(entity.Category)),
            new SqlParameter("@IsRequired", ToDbValue(entity.IsRequired))
        };

        return _sqlRepository.FromSqlFirstOrDefaultAsync(AppraisalQuestions_Update, parameters);
    }

    public override async Task<bool> DeleteAsync(int questionId)
    {
        var rowsAffected = await _sqlRepository.ExecuteSqlAsync(AppraisalQuestions_Delete, new SqlParameter("@QuestionID", questionId));
        return rowsAffected > 0;
    }

    private static object ToDbValue<T>(T? value)
        => value is null ? DBNull.Value : value!;
}
