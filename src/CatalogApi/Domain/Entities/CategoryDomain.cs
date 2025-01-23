namespace CatalogApi.Domain.Entities;

public class CategoryDomain : IBaseDomain {
  public long Id { get; set; }
  public string Name { get; set; } = null!;

  public ICollection<ProductDomain> Products { get; set; } = [];
}
