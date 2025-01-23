using CatalogApi.Dtos;
using CatalogApi.Errors;
using CatalogApi.Helpers.Validators;
using CatalogApi.Persistence.Repositories;
using CatalogApi.Helpers;
using System.Security.Claims;

namespace CatalogApi.Services;


public interface IUserService {
  Task<IEnumerable<UserResponse>> GetUsers();
  Task<UserResponseWithProducts> GetUser(long id);
  Task<IEnumerable<ProductResponse>> GetUserProducts(ClaimsPrincipal userClaim);
  Task<UserResponseWithProducts> AddUser(CreateUserDto user);
  Task<UserResponse> UpdateUser(UpdateUserDto user, ClaimsPrincipal userClaim);
  Task DeleteUser(long id);
}

public class UserService(IUserRepository repository, IProductRepository productRepository) : IUserService {
  private readonly IUserRepository _repository = repository;
  private readonly IProductRepository _productRepository = productRepository;

  public async Task<IEnumerable<UserResponse>> GetUsers() {
    var users = await _repository.GetAll();
    return users.Select(UserResponse.FromDomain);
  }

  public async Task<UserResponseWithProducts> GetUser(long id) {
    var user = await _repository.GetById(id)
      ?? throw new NotFoundException(UserServiceErrors.NotFounded.Value);
    return UserResponseWithProducts.FromDomain(user);
  }

  public async Task<IEnumerable<ProductResponse>> GetUserProducts(ClaimsPrincipal userClaim) {
    var userId = AuthenticationHelper.GetUserClaimId(userClaim);
    var products = await _productRepository.GetByUserId(userId);

    return products.Select(ProductResponse.FromDomain);
  }

  public async Task<UserResponseWithProducts> AddUser(CreateUserDto user) {
    var userValidator = new CreateUserDtoValidator(_repository);
    var validation = await userValidator.ValidateAsync(user);

    if (!validation.IsValid) {
      throw new NotFoundException(validation.Errors.First().ErrorMessage);
    }

    var userDomain = user.ToDomain();
    await _repository.Add(userDomain);
    return UserResponseWithProducts.FromDomain(userDomain);
  }

  public async Task<UserResponse> UpdateUser(UpdateUserDto user, ClaimsPrincipal userClaim) {
    var userId = AuthenticationHelper.GetUserClaimId(userClaim);

    var userValidator = new UpdateUserDtoValidator(_repository, userId);
    var validation = await userValidator.ValidateAsync(user);

    if (!validation.IsValid) {
      throw new BadRequestException(validation.Errors.First().ErrorMessage);
    }

    var userExists = await _repository.GetById(userId)
      ?? throw new NotFoundException(UserServiceErrors.NotFounded.Value);


    userExists.Email = user.Email;
    userExists.Name = user.Name;
    await _repository.Update(userExists);
    return UserResponse.FromDomain(userExists);
  }

  public async Task DeleteUser(long id) {
    var user = await _repository.GetById(id)
      ?? throw new NotFoundException(UserServiceErrors.NotFounded.Value);
    await _repository.Remove(user);
  }

}
