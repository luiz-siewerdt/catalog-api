using CatalogApi.Dtos;
using CatalogApi.Domain.Entities;

namespace CatalogApi.Tests.MockData;

public class ProductsMockData {
  public static ProductDomain GetById(long id = 1, long userId = 1) =>
    new ProductDomain {
      Id = id,
      Name = $"product {id}",
      Discount = 0,
      Price = 0,
      UserId = userId,
      User = UsersMockData.GetById(userId),
      Categories = []
    };

  public static IEnumerable<ProductDomain> GetAll(int qtde = 5) {
    var data = new List<ProductDomain>();
    for (var i = 1; i <= qtde; ++i) {
      data.Add(GetById(i));
    }
    return data;
  }

  public static ProductResponse GetProduct(long id = 1) =>
    ProductResponse.FromDomain(GetById(id));

  public static ProductResponseWithUser GetProductWithUser(long id = 1) =>
    ProductResponseWithUser.FromDomain(GetById(id));

  public static ProductResponseWithUserAndCategories GetProductWithUserAndCategories(long id = 1) =>
    ProductResponseWithUserAndCategories.FromDomain(GetById(id));

  public static IEnumerable<ProductResponseWithUser> GetProductsWithUser(int qtde = 5) {
    var products = GetAll(qtde);
    return products.Select(ProductResponseWithUser.FromDomain);
  }

  public static IEnumerable<ProductResponseWithUserAndCategories> GetProductsWithUserAndCategories(int qtde = 5) {
    var products = GetAll(qtde);
    return products.Select(ProductResponseWithUserAndCategories.FromDomain);
  }

  public static ProductResponseWithUser AddProductResponse() => new(1, "name", 10, 1, new(2, "user", "email"));
}
