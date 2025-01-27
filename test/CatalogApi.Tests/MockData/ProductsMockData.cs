using CatalogApi.Dtos;

namespace CatalogApi.Tests.MockData;

public class ProductsMockData {
  public static List<ProductResponseWithUser> GetProducts() =>
    new List<ProductResponseWithUser> {
      new (1, "Product 1", 10, 2, new(1, "José", "JoséEmail")),
      new (2, "Product 2", 20, 4, new(2, "John", "JohnEmail"))
    };

  public static ProductResponseWithUser AddProductResponse() => new(1, "name", 10, 1, new(2, "user", "email"));
}
