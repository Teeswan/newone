using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EPMS.Domain.Interfaces;

public interface IBaseRepository<T, TKey> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(TKey id);
    Task<T?> GetByIdFromDbAsync(TKey id);
    Task<T> CreateAsync(T entity);
    Task<T?> UpdateAsync(T entity);
    Task<bool> DeleteAsync(TKey id);
}
