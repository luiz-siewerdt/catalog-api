using Microsoft.AspNetCore.Mvc;
using CatalogApi.Tests.MockData;
using CatalogApi.Services;
using CatalogApi.Controllers;
using CatalogApi.Dtos;
using CatalogApi.Errors;
using System.Security.Claims;
using Moq;

namespace CatalogApi.Tests.Controllers;

public class TestUsersController {
  [Fact]
  public async Task UsersController_GetUsers_ShouldReturn200Status() {
    var expectedResponseData = UsersMockData.GetUsers();
    var usersService = new Mock<IUserService>();
    usersService.Setup(static _ => _.GetUsers()).ReturnsAsync(expectedResponseData);
    var sut = new UsersController(usersService.Object);

    var result = await sut.GetUsers();
    var okResult = Assert.IsType<OkObjectResult>(result.Result);
    var actualResponseData = Assert.IsType<List<UserResponse>>(okResult.Value);


    Assert.Equal(200, okResult.StatusCode);
    Assert.Equal(expectedResponseData.Count(), actualResponseData.Count);
  }

  [Fact]
  public async Task UserController_GetUserProducts_ShouldReturn200Status() {
    var expectedResponseData = UsersMockData.GetUserProducts();
    var userService = new Mock<IUserService>();
    userService.Setup(static _ => _.GetUserProducts(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(expectedResponseData);

    var sut = new UsersController(userService.Object);
    var result = await sut.GetUserProducts();
    var okResult = Assert.IsType<OkObjectResult>(result.Result);
    var actualResponseData = Assert.IsType<List<ProductResponse>>(okResult.Value);

    Assert.Equal(200, okResult.StatusCode);
    Assert.Equal(expectedResponseData.Count(), actualResponseData.Count);
  }

  [Fact]
  public async Task UserController_GetUser_ShouldReturn200Status() {
    var expectedResponseData = UsersMockData.GetUserWithProducts();
    var userService = new Mock<IUserService>();

    userService.Setup(static _ => _.GetUser(It.IsAny<long>())).ReturnsAsync(expectedResponseData);

    var sut = new UsersController(userService.Object);
    var result = await sut.GetUser(It.IsAny<long>());

    var okResult = Assert.IsType<OkObjectResult>(result.Result);
    var actualResponseData = Assert.IsType<UserResponseWithProducts>(okResult.Value);

    Assert.Equal(200, okResult.StatusCode);
    Assert.Equal(expectedResponseData.Email, actualResponseData.Email);
  }


  [Fact]
  public async Task UserController_GetUser_ShouldReturn404Status() {
    var expectedException = new NotFoundException(UserServiceErrors.NotFound);
    var userService = new Mock<IUserService>();

    userService.Setup(static _ => _.GetUser(It.IsAny<long>())).ThrowsAsync(expectedException);

    var sut = new UsersController(userService.Object);

    var actualException = await Assert.ThrowsAsync<NotFoundException>(async () => await sut.GetUser(It.IsAny<long>()));

    Assert.Equal(404, actualException.StatusCode);
    Assert.Equal(expectedException.StatusMessage, actualException.StatusMessage);
  }

  [Fact]
  public async Task UsersController_AddUser_ShouldReturn201Status() {
    var expectedResponseData = UsersMockData.GetUserWithProducts();
    var userService = new Mock<IUserService>();
    userService.Setup(static _ => _.AddUser(It.IsAny<CreateUserDto>())).ReturnsAsync(expectedResponseData);

    var sut = new UsersController(userService.Object);
    var result = await sut.AddUser(It.IsAny<CreateUserDto>());

    var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
    var actualResponseData = Assert.IsType<UserResponseWithProducts>(createdResult.Value);

    Assert.Equal(201, createdResult.StatusCode);
    Assert.Equal(expectedResponseData.Name, actualResponseData.Name);
  }


  [Fact]
  public async Task UsersController_AddUser_ShouldReturn400Status() {
    var userService = new Mock<IUserService>();
    var expectedException = new BadRequestException(UserServiceErrors.InvalidName);
    userService.Setup(static _ => _.AddUser(It.IsAny<CreateUserDto>())).ThrowsAsync(expectedException);

    var sut = new UsersController(userService.Object);
    var act = async () => await sut.AddUser(It.IsAny<CreateUserDto>());

    var actualException = await Assert.ThrowsAsync<BadRequestException>(act);

    Assert.Equal(400, actualException.StatusCode);
    Assert.Equal(expectedException.StatusMessage, actualException.StatusMessage);
  }

  [Fact]
  public async Task UsersController_UpdateUser_ShouldReturn200Status() {
    var expectedResponseData = UsersMockData.GetUser();
    var userService = new Mock<IUserService>();
    userService.Setup(static _ => _.UpdateUser(It.IsAny<UpdateUserDto>(), It.IsAny<ClaimsPrincipal>()))
      .ReturnsAsync(expectedResponseData);
    var sut = new UsersController(userService.Object);

    var result = await sut.UpdateUser(It.IsAny<UpdateUserDto>());
    var okObject = Assert.IsType<OkObjectResult>(result.Result);
    var actualResponseData = Assert.IsType<UserResponse>(okObject.Value);

    Assert.Equal(200, okObject.StatusCode);
    Assert.Equal(expectedResponseData.Name, actualResponseData.Name);
  }

  [Theory]
  [InlineData(400, typeof(BadRequestException), UserServiceErrors.InvalidName)]
  [InlineData(401, typeof(UnauthorizedException), "")]
  [InlineData(404, typeof(NotFoundException), UserServiceErrors.InvalidName)]
  public async Task UserController_UpdateUser_ShouldReturnBadStatus(
      int expectedStatusCode,
      Type exceptionType,
      string errorMessage) {
    var userService = new Mock<IUserService>();
    var expectedException = (Exception)Activator.CreateInstance(exceptionType, errorMessage)!;
    userService.Setup(static _ => _.UpdateUser(It.IsAny<UpdateUserDto>(), It.IsAny<ClaimsPrincipal>())).ThrowsAsync(expectedException);

    var sut = new UsersController(userService.Object);
    var act = async () => await sut.UpdateUser(It.IsAny<UpdateUserDto>());

    var actualException = await Assert.ThrowsAsync(exceptionType, act);

    Assert.Equal(expectedStatusCode, ((dynamic)actualException).StatusCode);
    if (errorMessage != null) {
      Assert.Equal(errorMessage, ((dynamic)actualException).StatusMessage);
    }
  }

  [Fact]
  public async Task UserController_DeleteUser_ShouldReturn204Status() {
    var userService = new Mock<IUserService>();

    var sut = new UsersController(userService.Object);

    var result = await sut.DeleteUser(It.IsAny<long>());
    var noContentResponse = Assert.IsType<NoContentResult>(result);

    Assert.Equal(204, noContentResponse.StatusCode);
  }

  [Fact]
  public async Task UserController_DeleteUser_ShouldReturn404Status() {
    var userService = new Mock<IUserService>();
    var expectedException = new NotFoundException(UserServiceErrors.NotFound);
    userService.Setup(static _ => _.DeleteUser(It.IsAny<long>())).ThrowsAsync(expectedException);

    var sut = new UsersController(userService.Object);
    var act = async () => await sut.DeleteUser(It.IsAny<long>());

    var actualException = await Assert.ThrowsAsync<NotFoundException>(act);

    Assert.Equal(404, actualException.StatusCode);
    Assert.Equal(expectedException.StatusMessage, actualException.StatusMessage);
  }

}
