using Microsoft.EntityFrameworkCore;
using CatalogApi.Domain.Entities;
using CatalogApi.Persistence.Data;

namespace CatalogApi.Persistence.Repositories;

public interface IUserRepository : IBaseRepository<UserDomain> {
  Task<UserDomain?> GetByEmail(string email);
  Task<bool> EmailAlredyInUse(string email, long id = 0);
}

public class UserRepository(CatalogApiContext context) : BaseRepository<UserDomain>(context), IUserRepository {
  public async Task<bool> EmailAlredyInUse(string email, long id = 0) {
    return await _context.Users
      .AnyAsync(e => e.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && e.Id != id);
  }

  public async Task<UserDomain?> GetByEmail(string email) {
    return await _context.Users.Where(e => e.Email == email).FirstOrDefaultAsync();
  }
}

