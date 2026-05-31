using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPMS.Domain.Interfaces;
using EPMS.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace EPMS.Infrastructure.Repositories;

public class SqlRepository<T, TKey> : BaseRepository<T, TKey>, ISqlRepository<T, TKey> where T : class
{
    public SqlRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<T>> FromSqlAsync(string storedProcedureName, params object[] parameters)
    {
        // Build command string with placeholders for each parameter
        var paramPlaceholders = parameters?.Select((p, i) => $"@p{i}").ToArray() ?? Array.Empty<string>();
        string commandText = $"EXEC {storedProcedureName}{(paramPlaceholders.Length > 0 ? " " + string.Join(", ", paramPlaceholders) : "")}";

        // Convert parameters to SqlParameter if they aren't already
        var sqlParams = parameters?.Select((p, i) =>
        {
            if (p is SqlParameter sqlParam)
            {
                // Ensure parameter name matches our placeholder
                sqlParam.ParameterName = $"@p{i}";
                return sqlParam;
            }
            // Create new SqlParameter if it's just a value
            return new SqlParameter($"@p{i}", p ?? DBNull.Value);
        }).ToArray() ?? Array.Empty<object>();

        return await _dbSet.FromSqlRaw(commandText, sqlParams).AsNoTracking().ToListAsync();
    }

    public async Task<T?> FromSqlFirstOrDefaultAsync(string storedProcedureName, params object[] parameters)
    {
        var results = await FromSqlAsync(storedProcedureName, parameters);
        return results.FirstOrDefault();
    }

    public async Task<int> ExecuteSqlAsync(string storedProcedureName, params object[] parameters)
    {
        // Build command string with placeholders for each parameter
        var paramPlaceholders = parameters?.Select((p, i) => $"@p{i}").ToArray() ?? Array.Empty<string>();
        string commandText = $"EXEC {storedProcedureName}{(paramPlaceholders.Length > 0 ? " " + string.Join(", ", paramPlaceholders) : "")}";

        // Convert parameters to SqlParameter if they aren't already
        var sqlParams = parameters?.Select((p, i) =>
        {
            if (p is SqlParameter sqlParam)
            {
                // Ensure parameter name matches our placeholder
                sqlParam.ParameterName = $"@p{i}";
                return sqlParam;
            }
            // Create new SqlParameter if it's just a value
            return new SqlParameter($"@p{i}", p ?? DBNull.Value);
        }).ToArray() ?? Array.Empty<object>();

        return await _context.Database.ExecuteSqlRawAsync(commandText, sqlParams);
    }
}
