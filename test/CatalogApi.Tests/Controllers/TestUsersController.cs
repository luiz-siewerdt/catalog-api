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
    var usersService = new Mock<IUserService>();
    usersService.Setup(static _ => _.GetUsers()).ReturnsAsync(UsersMockData.GetUsers());
    var sut = new UsersController(usersService.Object);

    var result = await sut.GetUsers();
    var okResult = Assert.IsType<OkObjectResult>(result.Result);
    var resultValue = Assert.IsType<List<UserResponse>>(okResult.Value);


    Assert.Equal(200, okResult.StatusCode);
    Assert.Equal(UsersMockData.GetUsers().Count(), resultValue.Count);
  }

  [Fact]
  public async Task UsersController_AddUser_ShouldReturn201Status() {
    var responseData = UsersMockData.GetUserWithProducts();
    var userService = new Mock<IUserService>();
    userService.Setup(static _ => _.AddUser(It.IsAny<CreateUserDto>())).ReturnsAsync(responseData);

    var sut = new UsersController(userService.Object);
    var result = await sut.AddUser(It.IsAny<CreateUserDto>());

    var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
    var resultValue = Assert.IsType<UserResponseWithProducts>(createdResult.Value);

    Assert.Equal(201, createdResult.StatusCode);
    Assert.Equal(responseData.Name, resultValue.Name);
  }

  [Fact]
  public async Task UsersController_AddUser_ShouldReturn400Status() {
    var userService = new Mock<IUserService>();
    var exception = new BadRequestException(UserServiceErrors.InvalidName.Value);
    userService.Setup(static _ => _.AddUser(It.IsAny<CreateUserDto>())).ThrowsAsync(exception);

    var sut = new UsersController(userService.Object);
    var act = async () => await sut.AddUser(It.IsAny<CreateUserDto>());

    var ex = await Assert.ThrowsAsync<BadRequestException>(act);

    Assert.Equal(400, ex.StatusCode);
    Assert.Equal(exception.StatusMessage, ex.StatusMessage);
  }

  [Fact]
  public async Task UsersController_UpdateUser_ShouldReturn200Status() {
    var responseData = UsersMockData.GetUser();
    var userService = new Mock<IUserService>();
    userService.Setup(static _ => _.UpdateUser(It.IsAny<UpdateUserDto>(), It.IsAny<ClaimsPrincipal>()))
      .ReturnsAsync(responseData);
    var sut = new UsersController(userService.Object);

    var result = await sut.UpdateUser(It.IsAny<UpdateUserDto>());

    var okObject = Assert.IsType<OkObjectResult>(result.Result);
    var objectResponse = Assert.IsType<UserResponse>(okObject.Value);

    Assert.Equal(200, okObject.StatusCode);
    Assert.Equal(responseData.Name, objectResponse.Name);
  }

  [Fact]
  public async Task UserController_UpdateUser_ShouldReturn400Status() {
    var userService = new Mock<IUserService>();
    var exception = new BadRequestException(UserServiceErrors.InvalidName.Value);
    userService.Setup(static _ => _.UpdateUser(It.IsAny<UpdateUserDto>(), It.IsAny<ClaimsPrincipal>())).ThrowsAsync(exception);

    var sut = new UsersController(userService.Object);
    var act = async () => await sut.UpdateUser(It.IsAny<UpdateUserDto>());

    var ex = await Assert.ThrowsAsync<BadRequestException>(act);

    Assert.Equal(400, ex.StatusCode);
    Assert.Equal(exception.StatusMessage, ex.StatusMessage);
  }

  [Fact]
  public async Task UserController_UpdateUser_ShouldReturn401Status() {
    var userService = new Mock<IUserService>();
    var exception = new UnauthorizedException();
    userService.Setup(static _ => _.UpdateUser(It.IsAny<UpdateUserDto>(), It.IsAny<ClaimsPrincipal>())).ThrowsAsync(exception);

    var sut = new UsersController(userService.Object);
    var act = async () => await sut.UpdateUser(It.IsAny<UpdateUserDto>());

    var ex = await Assert.ThrowsAsync<UnauthorizedException>(act);

    Assert.Equal(401, ex.StatusCode);
    Assert.Equal(exception.StatusMessage, ex.StatusMessage);
  }

  [Fact]
  public async Task UserController_UpdateUser_ShouldReturn404Status() {
    var userService = new Mock<IUserService>();
    var exception = new NotFoundException(UserServiceErrors.InvalidName.Value);
    userService.Setup(static _ => _.UpdateUser(It.IsAny<UpdateUserDto>(), It.IsAny<ClaimsPrincipal>())).ThrowsAsync(exception);

    var sut = new UsersController(userService.Object);
    var act = async () => await sut.UpdateUser(It.IsAny<UpdateUserDto>());

    var ex = await Assert.ThrowsAsync<NotFoundException>(act);

    Assert.Equal(404, ex.StatusCode);
    Assert.Equal(exception.StatusMessage, ex.StatusMessage);
  }

}
