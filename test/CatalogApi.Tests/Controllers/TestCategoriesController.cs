using Microsoft.AspNetCore.Mvc;
using CatalogApi.Tests.MockData;
using CatalogApi.Services;
using CatalogApi.Controllers;
using CatalogApi.Dtos;
using CatalogApi.Errors;
using Moq;

namespace CatalogApi.Tests.Controllers;

public class TestCategoriesController {
  [Fact]
  public async Task CategoriesController_GetCategories_ShouldReturn200Status() {
    var expectedResponseData = CategoriesMockData.GetCategories();
    var categoryService = new Mock<ICategoryService>();
    categoryService.Setup(static _ => _.GetCategories()).ReturnsAsync(expectedResponseData);

    var sut = new CategoriesController(categoryService.Object);
    var result = await sut.GetCategories();

    var okObject = Assert.IsType<OkObjectResult>(result.Result);
    var actualResponseData = Assert.IsType<List<CategoryResponse>>(okObject.Value);

    Assert.Equal(200, okObject.StatusCode);
    Assert.Equal(expectedResponseData.Count, actualResponseData.Count);
  }

  [Fact]
  public async Task CategoriesController_GetCategory_ShouldReturn200Status() {
    var expectedResponseData = CategoriesMockData.GetCategory();
    var categoryService = new Mock<ICategoryService>();
    categoryService.Setup(static _ => _.GetCategory(It.IsAny<long>()))
      .ReturnsAsync(expectedResponseData);

    var sut = new CategoriesController(categoryService.Object);
    var result = await sut.GetCategory(It.IsAny<long>());

    var okObject = Assert.IsType<OkObjectResult>(result.Result);
    var actualResponseData = Assert.IsType<CategoryResponse>(okObject.Value);

    Assert.Equal(200, okObject.StatusCode);
    Assert.Equal(expectedResponseData.Name, actualResponseData.Name);
  }

  [Fact]
  public async Task CategoriesController_GetCategory_ShouldReturn404Status() {
    var expectedException = new NotFoundException(CategoryServiceErrors.NotFound);
    var categoryService = new Mock<ICategoryService>();
    categoryService.Setup(static _ => _.GetCategory(It.IsAny<long>())).ThrowsAsync(expectedException);

    var sut = new CategoriesController(categoryService.Object);
    var act = async () => await sut.GetCategory(It.IsAny<long>());

    var actualException = await Assert.ThrowsAsync<NotFoundException>(act);

    Assert.Equal(404, actualException.StatusCode);
    Assert.Equal(expectedException.StatusMessage, actualException.StatusMessage);
  }

  [Fact]
  public async Task CategoriesController_AddCategory_ShouldReturn201Status() {
    var expectedResponseData = CategoriesMockData.GetCategory();
    var categoryService = new Mock<ICategoryService>();
    categoryService.Setup(static _ => _.AddCategory(It.IsAny<CreateCategoryDto>())).ReturnsAsync(expectedResponseData);

    var sut = new CategoriesController(categoryService.Object);
    var result = await sut.AddCategory(It.IsAny<CreateCategoryDto>());

    var createdAtActionObject = Assert.IsType<CreatedAtActionResult>(result.Result);
    var actualResponseData = Assert.IsType<CategoryResponse>(createdAtActionObject.Value);
  }

  [Theory]
  [InlineData(400, typeof(BadRequestException), CategoryServiceErrors.InvalidName)]
  public async Task CategoriesController_AddCategory_ShouldReturnErrorStatus(int statusCode, Type exceptionType, string message) {
    var expectedException = (ServiceException)Activator.CreateInstance(exceptionType, message)!;
    var categoryService = new Mock<ICategoryService>();
    categoryService.Setup(static _ => _.AddCategory(It.IsAny<CreateCategoryDto>()))
      .ThrowsAsync(expectedException);

    var sut = new CategoriesController(categoryService.Object);

    var act = async () => await sut.AddCategory(It.IsAny<CreateCategoryDto>());

    var actualException = await Assert.ThrowsAsync(exceptionType, act);

    Assert.Equal(statusCode, ((dynamic)actualException).StatusCode);

    if (message is not null) {
      Assert.Equal(message, ((dynamic)actualException).StatusMessage);
    }
  }

  [Fact]
  public async Task CategoriesController_UpdateCategory_ShouldReturn200Status() {
    var expectedResponseData = CategoriesMockData.GetCategory();
    var categoryService = new Mock<ICategoryService>();
    categoryService.Setup(static _ => _.UpdateCategory(It.IsAny<long>(), It.IsAny<UpdateCategoryDto>()))
      .ReturnsAsync(expectedResponseData);

    var sut = new CategoriesController(categoryService.Object);
    var result = await sut.UpdateCategory(It.IsAny<long>(), It.IsAny<UpdateCategoryDto>());

    var okObject = Assert.IsType<OkObjectResult>(result.Result);
    var actualResponseData = Assert.IsType<CategoryResponse>(okObject.Value);

    Assert.Equal(200, okObject.StatusCode);
    Assert.Equal(expectedResponseData.Name, actualResponseData.Name);
  }

  [Theory]
  [InlineData(400, typeof(BadRequestException), CategoryServiceErrors.InvalidName)]
  public async Task CategoriesController_UpdateCategory_ShouldReturnErrorStatus(int statusCode, Type exceptionType, string message) {
    var expectedException = (ServiceException)Activator.CreateInstance(exceptionType, message)!;
    var categoryService = new Mock<ICategoryService>();
    categoryService.Setup(static _ => _.UpdateCategory(It.IsAny<long>(), It.IsAny<UpdateCategoryDto>()))
      .ThrowsAsync(expectedException);

    var sut = new CategoriesController(categoryService.Object);
    var act = async () => await sut.UpdateCategory(It.IsAny<long>(), It.IsAny<UpdateCategoryDto>());

    var actualException = await Assert.ThrowsAsync(exceptionType, act);

    Assert.Equal(statusCode, ((dynamic)actualException).StatusCode);
    if (message is not null) {
      Assert.Equal(message, ((dynamic)actualException).StatusMessage);
    }
  }

  [Fact]
  public async Task CategoriesController_DeleteCategory_ShouldReturn204Status() {
    var categoryService = new Mock<ICategoryService>();

    var sut = new CategoriesController(categoryService.Object);
    var result = await sut.DeleteCategory(It.IsAny<long>());

    var okObject = Assert.IsType<NoContentResult>(result);

    Assert.Equal(204, okObject.StatusCode);
  }

  [Fact]
  public async Task CategoriesController_DeleteCategory_ShouldReturn404Status() {
    var expectedException = new NotFoundException(CategoryServiceErrors.NotFound);
    var categoryService = new Mock<ICategoryService>();
    categoryService.Setup(static _ => _.DeleteCategory(It.IsAny<long>())).ThrowsAsync(expectedException);

    var sut = new CategoriesController(categoryService.Object);
    var act = async () => await sut.DeleteCategory(It.IsAny<long>());

    var actualException = await Assert.ThrowsAsync<NotFoundException>(act);

    Assert.Equal(404, actualException.StatusCode);
    Assert.Equal(expectedException.StatusMessage, actualException.StatusMessage);
  }
}
