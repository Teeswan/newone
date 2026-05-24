using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using EPMS.Domain.Interfaces;
using EPMS.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace EPMS.Infrastructure.Repositories;

public class SqlRepository<T, TKey> : BaseRepository<T, TKey>, ISqlRepository<T, TKey> where T : class
{
    public SqlRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<T>> FromSqlAsync(string storedProcedureName, params object[] parameters)
    {
        var sql = FormattableStringFactory.Create($"EXEC {storedProcedureName} {string.Join(", ", parameters.Select((p, i) => $"{{{i}}}"))}", parameters);
        return await _dbSet.FromSqlInterpolated(sql).ToListAsync();
    }

    public async Task<T?> FromSqlFirstOrDefaultAsync(string storedProcedureName, params object[] parameters)
    {
        var sql = FormattableStringFactory.Create($"EXEC {storedProcedureName} {string.Join(", ", parameters.Select((p, i) => $"{{{i}}}"))}", parameters);
        var results = await _dbSet.FromSqlInterpolated(sql).ToListAsync();
        return results.FirstOrDefault();
    }

    public async Task<int> ExecuteSqlAsync(string storedProcedureName, params object[] parameters)
    {
        var sql = FormattableStringFactory.Create($"EXEC {storedProcedureName} {string.Join(", ", parameters.Select((p, i) => $"{{{i}}}"))}", parameters);
        return await _context.Database.ExecuteSqlInterpolatedAsync(sql);
    }
}
