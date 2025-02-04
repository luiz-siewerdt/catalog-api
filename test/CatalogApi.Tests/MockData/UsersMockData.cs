using CatalogApi.Dtos;
using CatalogApi.Domain.Entities;

namespace CatalogApi.Tests.MockData;

public class UsersMockData {

  public static UserDomain GetById(long id = 0) =>
    new UserDomain { Id = id, Email = $"user{id}@email", Name = $"user {id}", Password = "password", Products = [] };

  public static UserResponse GetUser(long id = 1) => UserResponse.FromDomain(GetById(id));

  public static UserResponseWithProducts GetUserWithProducts() => new(1, "user", "email", []);

  public static IEnumerable<UserResponse> GetUsers(int qtde = 5) {
    var usersResponse = new List<UserResponse>();
    for (int i = 1; i <= qtde; ++i) {
      usersResponse.Add(GetUser(i));
    }
    return usersResponse;
  }

  public static IEnumerable<UserDomain> GetAll(int qtde = 5) {
    var users = new List<UserDomain>();
    for (int i = 1; i <= qtde; ++i) {
      users.Add(GetById(i));
    }
    return users;
  }

  public static IEnumerable<ProductResponse> GetUserProducts(int qtde = 5) =>
    new List<ProductResponse> {
      new (1, "Produto 1", 10, 0, 4),
      new (2, "Produto 2", 40, 2, 10)
    };

}
