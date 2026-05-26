using System;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Infrastructure.Contexts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EPMS.Infrastructure.StoredProcedures;

namespace EPMS.Infrastructure.Repositories;

public class FormQuestionRepository : IFormQuestionRepository
{
    private readonly AppDbContext _context;
    private readonly DbSet<FormQuestion> _dbSet;

    public FormQuestionRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<FormQuestion>();
    }

    public async Task<IEnumerable<FormQuestion>> GetAllAsync()
    {
        return await _dbSet
            .Include(fq => fq.Question)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<FormQuestion>> GetByFormIdAsync(int formId)
    {
        return await _dbSet
            .Include(fq => fq.Question)
            .Where(fq => fq.FormId == formId)
            .OrderBy(fq => fq.SortOrder)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<FormQuestion?> GetByFormAndQuestionIdAsync(int formId, int questionId)
    {
        return await _dbSet
            .Include(fq => fq.Question)
            .FirstOrDefaultAsync(fq => fq.FormId == formId && fq.QuestionId == questionId);
    }

    public async Task<FormQuestion> CreateAsync(FormQuestion entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<FormQuestion?> UpdateAsync(FormQuestion entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int formId, int questionId)
    {
        var entity = await GetByFormAndQuestionIdAsync(formId, questionId);
        if (entity == null) return false;

        _dbSet.Remove(entity);
        var rowsAffected = await _context.SaveChangesAsync();
        return rowsAffected > 0;
    }
}
