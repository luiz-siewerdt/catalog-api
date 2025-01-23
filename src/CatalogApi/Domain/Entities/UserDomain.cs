
namespace CatalogApi.Domain.Entities;

public class UserDomain : IBaseDomain {
  public long Id { get; set; }
  public string Name { get; set; } = null!;
  public string Email { get; set; } = null!;
  public string Password { get; set; } = null!;

  public ICollection<ProductDomain> Products { get; set; } = [];
}
