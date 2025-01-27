using CatalogApi.Dtos;
using CatalogApi.Domain.Entities;

namespace CatalogApi.Tests.MockData;

public abstract class CategoriesMockData {
  public static List<CategoryResponse> GetCategories() =>
    new List<CategoryResponse> {
      new(1, "category 1"),
      new(2, "category 2")
    };

  public static CategoryResponse GetCategory() =>
    new(2, "category 2");

  public static List<CategoryDomain> GetAll() => new List<CategoryDomain>{
      new CategoryDomain { Id = 1, Name = "category 1", Products = [] },
      new CategoryDomain { Id = 2, Name = "category 2", Products = [] },
      new CategoryDomain { Id = 3, Name = "category 3", Products = [] },
    };

  public static CategoryDomain GetById() =>
    new CategoryDomain { Id = 1, Name = "category 1", Products = [] };
}
