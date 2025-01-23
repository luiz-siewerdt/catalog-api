using System.Linq.Expressions;
using CatalogApi.Domain.Entities;
using CatalogApi.Persistence.Data;
using Microsoft.EntityFrameworkCore;
namespace CatalogApi.Persistence.Repositories;

public interface IBaseRepository<T> where T : IBaseDomain {
  Task<IEnumerable<T>> GetAll();
  Task<T?> GetById(long id, params Expression<Func<T, object>>[] includeFuncs);
  Task Add(T entity);
  Task Remove(T entity);
  Task Update(T entity);
  Task<bool> Exists(long id);
}

public abstract class BaseRepository<T>(CatalogApiContext context) : IBaseRepository<T> where T : class, IBaseDomain {
  protected CatalogApiContext _context = context;

  public virtual async Task Add(T entity) {
    await _context.Set<T>().AddAsync(entity);
    await _context.SaveChangesAsync();
  }

  public virtual async Task<bool> Exists(long id) {
    return (await _context.Set<T>().FindAsync(id)) != null;
  }

  public virtual async Task<IEnumerable<T>> GetAll() {
    return await _context.Set<T>().ToListAsync();
  }

  public virtual async Task<T?> GetById(long id, params Expression<Func<T, object>>[] includeFuncs) {
    var includeList = includeFuncs.ToList();

    if (includeList.Count == 0) {
      return await _context.Set<T>().FindAsync(id);
    }

    var query = _context.Set<T>().AsQueryable();

    foreach (var include in includeList) {
      query = query.Include(include);
    }

    return await query.FirstOrDefaultAsync(e => e.Id == id);
  }

  public virtual async Task Remove(T entity) {
    _context.Set<T>().Remove(entity);
    await _context.SaveChangesAsync();
  }

  public virtual async Task Update(T entity) {
    _context.Set<T>().Update(entity);
    await _context.SaveChangesAsync();
  }
}
