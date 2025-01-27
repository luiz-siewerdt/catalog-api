using CatalogApi.Dtos;

namespace CatalogApi.Tests.MockData;

public class UsersMockData {
  public static UserResponse GetUser() => new(1, "name", "email");

  public static UserResponseWithProducts GetUserWithProducts() => new(1, "user", "email", []);

  public static IEnumerable<UserResponse> GetUsers() =>
     new List<UserResponse> {
      new (1, "Jão", "EmailJão"),
      new (2, "carlos", "emailcarlos")
  };

  public static IEnumerable<ProductResponse> GetUserProducts() =>
    new List<ProductResponse> {
      new (1, "Produto 1", 10, 0, 4),
      new (2, "Produto 2", 40, 2, 10)
    };
}
