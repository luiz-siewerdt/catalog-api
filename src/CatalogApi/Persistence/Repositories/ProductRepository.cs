using Microsoft.EntityFrameworkCore;
using CatalogApi.Domain.Entities;
using CatalogApi.Persistence.Data;
namespace CatalogApi.Persistence.Repositories;

public interface IProductRepository : IBaseRepository<ProductDomain> {
  Task<IEnumerable<ProductDomain>> GetAllWithUsers();
  Task<IEnumerable<ProductDomain>> GetByUserId(long userId);
  Task<ProductDomain?> GetByIdWithCategories(long id);
  Task<IEnumerable<ProductDomain>> GetProductsByCategories(List<string> categoryNames);
}

public class ProductRepository(CatalogApiContext context) : BaseRepository<ProductDomain>(context), IProductRepository {

  public async Task<ProductDomain?> GetByIdWithCategories(long id) {
    return await _context.Products.Include(static e => e.Categories)
      .FirstOrDefaultAsync(e => e.Id == id);
  }

  public async Task<IEnumerable<ProductDomain>> GetByUserId(long userId) {
    return await _context.Products.Where(e => e.UserId == userId)
      .Include(e => e.User)
      .ToListAsync();
  }

  public async Task<IEnumerable<ProductDomain>> GetAllWithUsers() {
    return await _context.Products.Include(static e => e.User).ToListAsync();
  }

  public async Task<IEnumerable<ProductDomain>> GetProductsByCategories(List<string> categoryNames) {
    return await _context.Products
      .Where(e => e.Categories
          .Select(e => e.Name)
          .Intersect(categoryNames).Count() == categoryNames.Count)
      .Include(e => e.User)
      .ToListAsync();
  }

}
