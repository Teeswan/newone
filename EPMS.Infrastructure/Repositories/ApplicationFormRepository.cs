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
/// Repository for application forms, implemented with stored procedure support.
/// </summary>
public class ApplicationFormRepository : BaseRepository<ApplicationForm, int>, IAppraisalFormRepository
{
    private readonly ISqlRepository<ApplicationForm, int> _sqlRepository;

    public ApplicationFormRepository(AppDbContext context, ISqlRepository<ApplicationForm, int> sqlRepository) : base(context)
    {
        _sqlRepository = sqlRepository ?? throw new ArgumentNullException(nameof(sqlRepository));
    }

    public override Task<IEnumerable<ApplicationForm>> GetAllAsync()
    {
        return _sqlRepository.FromSqlAsync(ApplicationForms_GetAll);
    }

    public override Task<ApplicationForm?> GetByIdAsync(int formId)
    {
        return _sqlRepository.FromSqlFirstOrDefaultAsync(ApplicationForms_GetById, new SqlParameter("@FormID", formId));
    }

    public override async Task<ApplicationForm> CreateAsync(ApplicationForm entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var parameters = new object[]
        {
            new SqlParameter("@FormName", ToDbValue(entity.FormName)),
            new SqlParameter("@IsActive", ToDbValue(entity.IsActive))
        };

        var result = await _sqlRepository.FromSqlFirstOrDefaultAsync(ApplicationForms_Create, parameters);
        return result ?? throw new InvalidOperationException("Failed to create appraisal form.");
    }

    public override Task<ApplicationForm?> UpdateAsync(ApplicationForm entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var parameters = new object[]
        {
            new SqlParameter("@FormID", entity.FormId),
            new SqlParameter("@FormName", ToDbValue(entity.FormName)),
            new SqlParameter("@IsActive", ToDbValue(entity.IsActive))
        };

        return _sqlRepository.FromSqlFirstOrDefaultAsync(ApplicationForms_Update, parameters);
    }

    public override async Task<bool> DeleteAsync(int formId)
    {
        var rowsAffected = await _sqlRepository.ExecuteSqlAsync(ApplicationForms_Delete, new SqlParameter("@FormID", formId));
        return rowsAffected > 0;
    }

    private static object ToDbValue<T>(T? value)
        => value is null ? DBNull.Value : value!;
}
