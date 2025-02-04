using CatalogApi.Domain.Entities;
using CatalogApi.Tests.MockData;
using CatalogApi.Tests.Utils;
using CatalogApi.Services;
using CatalogApi.Persistence.Repositories;
using CatalogApi.Dtos;
using CatalogApi.Errors;
using System.Linq.Expressions;
using System.Security.Claims;
using Moq;

namespace CatalogApi.Tests.Services;

// PERF: separate test methods by class

public class TestUserService {
  private readonly Mock<IUserRepository> userRepository;
  private readonly Mock<IProductRepository> productRepository;
  private readonly UserResponseCompare userResponseCompare = new();
  private readonly UserResponseWithProductsCompare userResponseWithProductsCompare = new();

  public TestUserService() {
    productRepository = new Mock<IProductRepository>()!;
    userRepository = new Mock<IUserRepository>()!;
  }

  [Fact]
  public async Task UserService_GetUsers_ShouldReturnUserResponseList() {
    var expectedResposeData = UsersMockData.GetUsers();
    userRepository.Setup(static _ => _.GetAll()).ReturnsAsync(UsersMockData.GetAll());

    var sut = new UserService(userRepository.Object, productRepository.Object);

    var result = await sut.GetUsers();

    Assert.Equal(expectedResposeData, result, userResponseCompare);

    userRepository.Verify(static _ => _.GetAll(), Times.Once());
  }

  [Fact]
  public async Task UserService_GetUser_ShouldReturnUserResponseWithProducts() {
    var expectedResponseData = UsersMockData.GetUserWithProducts();
    userRepository.Setup(static _ => _.GetById(
        It.IsAny<long>(),
        It.IsAny<Expression<Func<UserDomain, object>>>()))
      .ReturnsAsync(UsersMockData.GetById(1));

    var sut = new UserService(userRepository.Object, productRepository.Object);

    var actualResponseData = await sut.GetUser(expectedResponseData.Id);

    Assert.Equal(expectedResponseData.Id, actualResponseData.Id);

    userRepository.Verify(_ => _.GetById(
      It.Is<long>(id => id == expectedResponseData.Id),
      It.IsAny<Expression<Func<UserDomain, object>>>()
    ), Times.Once());
  }

  [Fact]
  public async Task UserService_GetUser_ShouldThrowNotFoundException() {
    var expectedException = new NotFoundException(UserServiceErrors.NotFound);
    userRepository.Setup(static _ => _.GetById(It.IsAny<long>(), It.IsAny<Expression<Func<UserDomain, object>>>()))
      .ReturnsAsync((UserDomain?)null);

    var sut = new UserService(userRepository.Object, productRepository.Object);

    var actualException = await Assert.ThrowsAsync<NotFoundException>(async () => await sut.GetUser(2));
    Assert.Equal(expectedException.StatusMessage, actualException.StatusMessage);
    userRepository.Verify(static _ => _.GetById(It.IsAny<long>(), It.IsAny<Expression<Func<UserDomain, object>>>()), Times.Once());
  }

  [Fact]
  public async Task UserService_AddUser_ShouldCreateSuccessfully() {
    var createUser = new CreateUserDto("name", "email@email.com", "password", "password");
    var expectedResponseData = CreateUserToUserResponseWithProducts(createUser, 1);

    userRepository.Setup(static _ => _.Add(It.IsAny<UserDomain>()))
      .Callback<UserDomain>(static u => u.Id = 1);

    var sut = new UserService(userRepository.Object, productRepository.Object);

    var actualResponseData = await sut.AddUser(createUser);

    Assert.NotNull(actualResponseData);
    Assert.Equal(expectedResponseData, actualResponseData, userResponseWithProductsCompare);

    userRepository.Verify(_ => _.Add(It.Is<UserDomain>(u =>
      u.Name == createUser.Name && u.Email == createUser.Email
    )), Times.Once());
  }

  [Fact]
  public async Task UserService_AddUser_ShouldThrowBadException() {
    var createUserDto = new CreateUserDto("", "email@email", "password", "password");
    var expectedException = new BadRequestException(UserServiceErrors.InvalidName);
    userRepository.Setup(static _ => _.EmailAlredyInUse(It.IsAny<string>(), It.IsAny<long>()))
      .ReturnsAsync(false);

    var sut = new UserService(userRepository.Object, productRepository.Object);

    var actualException = await Assert.ThrowsAsync<BadRequestException>(async () => await sut.AddUser(createUserDto));

    Assert.Equal(400, actualException.StatusCode);
    Assert.Equal(expectedException.StatusMessage, actualException.StatusMessage);

    userRepository.Verify(static _ => _.EmailAlredyInUse(It.IsAny<string>(), It.IsAny<long>()), Times.Once());
  }

  [Fact]
  public async Task UserService_UpdateUser_ShouldUpdateSuccessfully() {
    var claimsPrincipal = GenerateClaimsPrincipal.Generate(1);
    var updateUserDto = new UpdateUserDto("name", "update@email");
    userRepository.Setup(static _ => _.GetById(It.IsAny<long>())).ReturnsAsync(UsersMockData.GetById(1));
    userRepository.Setup(static _ => _.Update(It.IsAny<UserDomain>())).Callback<UserDomain>(e => {
      e.Name = updateUserDto.Name;
      e.Email = updateUserDto.Email;
    });

    var sut = new UserService(userRepository.Object, productRepository.Object);

    var result = await sut.UpdateUser(updateUserDto, claimsPrincipal);

    Assert.Equal(updateUserDto.Name, result.Name);
    Assert.Equal(updateUserDto.Email, result.Email);

    userRepository.Verify(_ => _.GetById(It.Is<long>(id => id == 1)), Times.Once());
    userRepository.Verify(_ => _.Update(It.Is<UserDomain>(
            u => u.Name == updateUserDto.Name && u.Email == updateUserDto.Email
            )), Times.Once());
  }

  [Fact]
  public async Task UserService_UpdateUser_ShouldThrowBadRequestException() {
    var expectedException = new BadRequestException(UserServiceErrors.InvalidName);
    var updateUserDto = new UpdateUserDto("", "update@email");
    var claimsPrincipal = GenerateClaimsPrincipal.Generate(1);


    var sut = new UserService(userRepository.Object, productRepository.Object);

    var actualException = await Assert.ThrowsAsync<BadRequestException>(
        async () => await sut.UpdateUser(updateUserDto, claimsPrincipal));

    Assert.Equal(expectedException.StatusMessage, actualException.StatusMessage);

    userRepository.Verify(static _ => _.GetById(It.IsAny<long>()), Times.Never());
    userRepository.Verify(static _ => _.Update(It.IsAny<UserDomain>()), Times.Never());
  }

  [Fact]
  public async Task UserService_UpdateUser_ShouldThrowUnauthorizedException() {
    var expectedException = new UnauthorizedException();
    var updateUserDto = new UpdateUserDto("", "update@email");


    var sut = new UserService(userRepository.Object, productRepository.Object);

    var actualException = await Assert.ThrowsAsync<UnauthorizedException>(
        async () => await sut.UpdateUser(updateUserDto, new ClaimsPrincipal()));

    Assert.Equal(expectedException.StatusMessage, actualException.StatusMessage);

    userRepository.Verify(static _ => _.GetById(It.IsAny<long>()), Times.Never());
    userRepository.Verify(static _ => _.Update(It.IsAny<UserDomain>()), Times.Never());
  }

  [Fact]
  public async Task UserService_UpdateUser_ShouldThrowNotFoundException() {
    var expectedException = new NotFoundException(UserServiceErrors.NotFound);
    var updateUserDto = new UpdateUserDto("usr", "update@email");
    var claimsPrincipal = GenerateClaimsPrincipal.Generate(1);

    userRepository.Setup(static _ => _.GetById(It.IsAny<long>())).ReturnsAsync((UserDomain?)null);

    var sut = new UserService(userRepository.Object, productRepository.Object);

    var actualException = await Assert.ThrowsAsync<NotFoundException>(
        async () => await sut.UpdateUser(updateUserDto, claimsPrincipal));

    Assert.Equal(expectedException.StatusMessage, actualException.StatusMessage);

    userRepository.Verify(_ => _.GetById(It.IsAny<long>()), Times.Once());
    userRepository.Verify(static _ => _.Update(It.IsAny<UserDomain>()), Times.Never());
  }


  // TODO: test delete user

  private static UserResponseWithProducts CreateUserToUserResponseWithProducts(CreateUserDto createUser, long id) {
    var userDomain = new UserDomain { Id = id, Name = createUser.Name, Email = createUser.Email, Password = createUser.Password, Products = [] };
    return UserResponseWithProducts.FromDomain(userDomain);
  }
}

public class UserResponseCompare : IEqualityComparer<UserResponse> {
  public bool Equals(UserResponse? x, UserResponse? y) {
    if (x == null || y == null) {
      return false;
    }
    return x.Id == y.Id && x.Name == y.Name && x.Email == y.Email;
  }

  public int GetHashCode(UserResponse obj) {
    return HashCode.Combine(obj.Id, obj.Name);
  }
}

public class UserResponseWithProductsCompare : IEqualityComparer<UserResponseWithProducts> {
  public bool Equals(UserResponseWithProducts? x, UserResponseWithProducts? y) {
    if (x == null || y == null) {
      return false;
    }
    return x.Id == y.Id && x.Name == y.Name && x.Email == y.Email && x.Products.Count() == y.Products.Count();
  }

  public int GetHashCode(UserResponseWithProducts obj) {
    return HashCode.Combine(obj.Id, obj.Name);
  }
}
