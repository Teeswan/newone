using System.Collections.Generic;
using System.Threading.Tasks;

namespace EPMS.Domain.Interfaces;

public interface ISqlRepository<T, TKey> : IBaseRepository<T, TKey> where T : class
{
    Task<IEnumerable<T>> FromSqlAsync(string storedProcedureName, params object[] parameters);
    Task<T?> FromSqlFirstOrDefaultAsync(string storedProcedureName, params object[] parameters);
    Task<int> ExecuteSqlAsync(string storedProcedureName, params object[] parameters);
}
