using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace MES.Infrastructure.Persistence.Repositories;

public interface IRepository<T> where T : class
{
    MesDbContext GetDbContext();
    Task<T?> GetByIdAsync(object id);
    Task<List<T>> GetAllAsync();
    Task<List<T>> GetWhereAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
}

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly MesDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(MesDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public MesDbContext GetDbContext() => _context;

    public virtual async Task<T?> GetByIdAsync(object id) =>
        await _dbSet.FindAsync(id);

    public virtual async Task<List<T>> GetAllAsync() =>
        await _dbSet.ToListAsync();

    public virtual async Task<List<T>> GetWhereAsync(Expression<Func<T, bool>> predicate) =>
        await _dbSet.Where(predicate).ToListAsync();

    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate) =>
        await _dbSet.AnyAsync(predicate);

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null) =>
        predicate == null ? await _dbSet.CountAsync() : await _dbSet.CountAsync(predicate);
}
