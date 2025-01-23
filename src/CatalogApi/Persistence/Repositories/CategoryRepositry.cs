
using Microsoft.EntityFrameworkCore;
using CatalogApi.Persistence.Data;
using CatalogApi.Domain.Entities;

namespace CatalogApi.Persistence.Repositories;

public interface ICategoryRepository : IBaseRepository<CategoryDomain> {
  Task<CategoryDomain?> GetByName(string name);
  Task<bool> NameAlreadyInUse(string name, long id = 0);
}

public class CategoryRepository(CatalogApiContext context) : BaseRepository<CategoryDomain>(context), ICategoryRepository {
  public async Task<CategoryDomain?> GetByName(string name) {
    return await _context.Categories
      .FirstOrDefaultAsync(e => EF.Functions.ILike(e.Name, name));
  }

  public async Task<bool> NameAlreadyInUse(string name, long id = 0) {
    return await _context.Categories
      .AnyAsync(e => e.Id != id && EF.Functions.ILike(e.Name, name));
  }
}
