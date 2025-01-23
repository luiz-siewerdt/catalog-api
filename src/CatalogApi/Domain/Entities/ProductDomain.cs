namespace CatalogApi.Domain.Entities;

public class ProductDomain : IBaseDomain {
  public long Id { get; set; }
  public string Name { get; set; } = null!;
  public float Price { get; set; }
  public float Discount { get; set; }

  public ICollection<CategoryDomain> Categories { get; set; } = [];

  public long UserId { get; set; }
  public UserDomain User { get; set; } = null!;
}
