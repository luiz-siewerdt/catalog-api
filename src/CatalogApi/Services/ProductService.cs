using System.Security.Claims;
using CatalogApi.Persistence.Repositories;
using CatalogApi.Errors;
using CatalogApi.Helpers;
using CatalogApi.Helpers.Validators;

using CatalogApi.Dtos;
namespace CatalogApi.Services;

public interface IProductService {
  Task<IEnumerable<ProductResponseWithUser>> GetProducts();
  Task<IEnumerable<ProductResponseWithUser>> GetProductsByCategories(List<string> categoryNames);
  Task<ProductResponseWithUserAndCategories> GetProduct(long id);
  Task<ProductResponseWithUser> AddProduct(CreateProductDto product, ClaimsPrincipal userClaim);
  Task AddProductCategory(ICollection<string> categoryNames, long productId, ClaimsPrincipal userClaim);
  Task<ProductResponse> UpdateProduct(long productId, UpdateProductDto product, ClaimsPrincipal userClaim);
  Task DeleteProduct(long id, ClaimsPrincipal userClaim);
}

public class ProductService(IProductRepository repository, IUserRepository userRepository, ICategoryRepository categoryRepository) : IProductService {
  private readonly IProductRepository _repository = repository;
  private readonly IUserRepository _userRepository = userRepository;
  private readonly ICategoryRepository _categoryRepository = categoryRepository;

  public async Task<IEnumerable<ProductResponseWithUser>> GetProducts() {
    var products = await _repository.GetAllWithUsers();
    return products.Select(ProductResponseWithUser.FromDomain);
  }

  public async Task<ProductResponseWithUserAndCategories> GetProduct(long id) {
    var product = await _repository.GetById(id, static e => e.Categories, static e => e.User)
      ?? throw new NotFoundException(ProductServiceErrors.NotFound);
    Console.WriteLine(product.UserId);
    return ProductResponseWithUserAndCategories.FromDomain(product);
  }

  public async Task<ProductResponseWithUser> AddProduct(CreateProductDto product, ClaimsPrincipal userClaim) {
    var userId = AuthenticationHelper.GetUserClaimId(userClaim);

    var user = await _userRepository.GetById(userId)
      ?? throw new NotFoundException(UserServiceErrors.NotFound);

    var productValidator = new CreateProductDtoValidator();
    var validation = await productValidator.ValidateAsync(product);

    if (!validation.IsValid) {
      throw new BadRequestException(validation.Errors.First().ErrorMessage);
    }

    var productDomain = product.ToDomain();
    productDomain.User = user;
    productDomain.UserId = userId;

    await _repository.Add(productDomain);
    return ProductResponseWithUser.FromDomain(productDomain);
  }

  public async Task AddProductCategory(ICollection<string> categoryNames, long productId, ClaimsPrincipal userClaim) {
    var userId = AuthenticationHelper.GetUserClaimId(userClaim);
    var product = await _repository.GetByIdWithCategories(productId)
      ?? throw new NotFoundException(ProductServiceErrors.NotFound);

    if (product.UserId != userId) {
      throw new UnauthorizedException();
    }

    foreach (var categoryName in categoryNames) {
      var categoryExists = await _categoryRepository.GetByName(categoryName)
        ?? throw new BadRequestException($"category with name {categoryName} not founded");
      if (!product.Categories.Contains(categoryExists)) {
        product.Categories.Add(categoryExists);
      }
    }

    await _repository.Update(product);
  }

  public async Task<ProductResponse> UpdateProduct(long productId, UpdateProductDto product, ClaimsPrincipal userClaim) {
    var productDomain = await _repository.GetById(productId)
      ?? throw new NotFoundException(ProductServiceErrors.NotFound);
    var userId = AuthenticationHelper.GetUserClaimId(userClaim);

    if (productDomain.UserId != userId) {
      throw new UnauthorizedException();
    }

    var productValidator = new UpdateProductDtoValidator();
    var validation = await productValidator.ValidateAsync(product);

    if (!validation.IsValid) {
      throw new BadRequestException(validation.Errors.First().ErrorMessage);
    }
    productDomain.Name = product.Name;
    productDomain.Discount = product.Discount;
    productDomain.Price = product.Price;

    await _repository.Update(productDomain);
    return ProductResponse.FromDomain(productDomain);
  }

  public async Task DeleteProduct(long id, ClaimsPrincipal userClaim) {
    var productExists = await _repository.GetById(id)
      ?? throw new NotFoundException(ProductServiceErrors.NotFound);
    var userId = AuthenticationHelper.GetUserClaimId(userClaim);
    if (productExists.UserId != userId) {
      throw new UnauthorizedException();
    }
    await _repository.Remove(productExists);
  }

  public async Task<IEnumerable<ProductResponseWithUser>> GetProductsByCategories(List<string> categoryNames) {
    var products = await _repository.GetProductsByCategories(categoryNames);
    return products.Select(ProductResponseWithUser.FromDomain);
  }
}

