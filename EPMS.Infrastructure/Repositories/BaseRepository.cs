using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPMS.Domain.Interfaces;
using EPMS.Infrastructure.Contexts;
using EPMS.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EPMS.Infrastructure.Repositories;

/// <summary>
/// Generic repository for basic CRUD operations using EF Core.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
/// <typeparam name="TKey">The entity primary key type.</typeparam>
public class BaseRepository<T, TKey> : IBaseRepository<T, TKey> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public BaseRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<T>();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(TKey id)
    {
        if (id == null)
        {
            throw new ArgumentNullException(nameof(id));
        }

        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<T?> UpdateAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        // Get the primary key property name
        var keyProperty = _context.Model.FindEntityType(typeof(T))?.FindPrimaryKey()?.Properties.FirstOrDefault();
        if (keyProperty != null)
        {
            var keyValue = keyProperty.PropertyInfo?.GetValue(entity);
            if (keyValue != null)
            {
                // Check if an entity with the same ID is already being tracked
                var trackedEntity = _dbSet.Local.FirstOrDefault(e => 
                    Equals(keyProperty.PropertyInfo?.GetValue(e), keyValue));

                if (trackedEntity != null)
                {
                    // If the tracked entity is a DIFFERENT instance than the one we're updating
                    if (!ReferenceEquals(trackedEntity, entity))
                    {
                        // Detach the existing tracked entity so we can track the new one
                        _context.Entry(trackedEntity).State = EntityState.Detached;
                    }
                }
            }
        }

        var entry = _context.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
            entry.State = EntityState.Modified;
        }
        else
        {
            entry.State = EntityState.Modified;
        }
        
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<bool> DeleteAsync(TKey id)
    {
        if (id == null)
        {
            throw new ArgumentNullException(nameof(id));
        }

        var entity = await GetByIdAsync(id);
        if (entity == null)
        {
            return false;
        }

        if (entity is ISoftDelete softDeleteEntity)
        {
            softDeleteEntity.IsDeleted = true;
            _context.Entry(entity).State = EntityState.Modified;
        }
        else
        {
            _dbSet.Remove(entity);
        }

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex)
        {
            throw HandleDbUpdateException(ex, entity);
        }
    }

    private Exception HandleDbUpdateException(DbUpdateException ex, T entity)
    {
        var innerException = ex.InnerException;
        if (innerException?.Message.Contains("REFERENCE constraint", StringComparison.OrdinalIgnoreCase) == true)
        {
            var primaryKeyProperty = _dbSet.EntityType.FindPrimaryKey()?.Properties.FirstOrDefault();
            var entityId = primaryKeyProperty != null
                ? entity.GetType().GetProperty(primaryKeyProperty.Name)?.GetValue(entity) ?? 0
                : 0;

            return new RelatedEntityExistsException(
                typeof(T).Name,
                entityId,
                "related",
                0);
        }

        return new EntityConstraintException($"Error deleting {typeof(T).Name}: {ex.Message}", ex);
    }
}
