using CatalogApi.Domain.Entities;

namespace CatalogApi.Dtos;

public record ProductDto(string Name, float Price, float Discount, long UserId = 0, long Id = 0);

public record CreateProductDto(string Name, float Price, float Discount)
  : ProductDto(Name, Price, Discount);

public record UpdateProductDto(string Name, float Price, float Discount)
  : ProductDto(Name, Price, Discount);

public record ProductResponse(long Id, string Name, float Price, float Discount, long UserId) {
  public static ProductResponse FromDomain(ProductDomain productDomain) {
    return new ProductResponse(productDomain.Id, productDomain.Name, productDomain.Price, productDomain.Discount, productDomain.UserId);
  }
}

public record ProductResponseWithUser(long Id, string Name, float Price, float Discount, UserResponse User) {
  public static ProductResponseWithUser FromDomain(ProductDomain productDomain) {
    var userResponse = UserResponse.FromDomain(productDomain.User);
    return new ProductResponseWithUser(productDomain.Id, productDomain.Name, productDomain.Price, productDomain.Discount, userResponse);
  }
}

public record ProductResponseWithUserAndCategories(long Id, string Name, float Price, float Discount, UserResponse User, IEnumerable<CategoryResponse> Categories) {
  public static ProductResponseWithUserAndCategories FromDomain(ProductDomain productDomain) {
    var userResponse = productDomain.User is not null
      ? UserResponse.FromDomain(productDomain.User)
      : UserResponse.FromDomain(new UserDomain { Email = "", Id = 3, Name = "nam", Password = "asddas", Products = [] });

    var categoriesResponse = productDomain.Categories.Select(CategoryResponse.FromDomain);
    return new ProductResponseWithUserAndCategories(productDomain.Id, productDomain.Name, productDomain.Price, productDomain.Discount, userResponse, categoriesResponse);
  }
}

public static class ProductDtoExtensions {
  public static ProductDomain ToDomain(this ProductDto productDto) {
    return new ProductDomain {
      Id = productDto.Id,
      Name = productDto.Name,
      Price = productDto.Price,
      Discount = productDto.Discount,
      UserId = productDto.UserId
    };
  }
}
