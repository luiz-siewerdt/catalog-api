using CatalogApi.Domain.Entities;
namespace CatalogApi.Dtos;

public record UserDto(string Name, string Email, string Password, long Id = 0);

public record CreateUserDto(string Name, string Email, string Password, string ConfPassword)
  : UserDto(Name, Email, Password);

public record UpdateUserDto(string Name, string Email)
  : UserDto(Name, Email, "");

public record UserResponse(long Id, string Name, string Email) {
  public static UserResponse FromDomain(UserDomain userDomain) {
    return new UserResponse(userDomain.Id, userDomain.Name, userDomain.Email);
  }
}

public record UserResponseWithProducts(long Id, string Name, string Email, IEnumerable<ProductResponse> Products) {
  public static UserResponseWithProducts FromDomain(UserDomain userDomain) {
    var productsDomain = userDomain.Products.Select(ProductResponse.FromDomain);
    return new UserResponseWithProducts(userDomain.Id, userDomain.Name, userDomain.Email, productsDomain);
  }
}

public static class UserDtoExtension {
  public static UserDomain ToDomain(this UserDto userDto) {
    return new UserDomain {
      Id = userDto.Id,
      Name = userDto.Name,
      Email = userDto.Email,
      Password = userDto.Password
    };
  }
}
