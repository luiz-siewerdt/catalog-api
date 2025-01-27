using Microsoft.AspNetCore.Mvc;
using CatalogApi.Tests.MockData;
using CatalogApi.Services;
using CatalogApi.Controllers;
using CatalogApi.Dtos;
using CatalogApi.Errors;
using System.Security.Claims;
using Moq;

namespace CatalogApi.Tests.Controllers;

public class TestProductsController {
  [Fact]
  public async Task ProductsController_GetProducts_ShouldReturn200Status() {
    var expectedResponseData = ProductsMockData.GetProducts();
    var productService = new Mock<IProductService>();
    productService.Setup(static _ => _.GetProducts()).ReturnsAsync(expectedResponseData);

    var sut = new ProductsController(productService.Object);
    var result = await sut.GetProducts();

    var okResult = Assert.IsType<OkObjectResult>(result.Result);
    var actualResponseData = Assert.IsType<List<ProductResponseWithUser>>(okResult.Value);

    Assert.Equal(200, okResult.StatusCode);
    Assert.Equal(expectedResponseData.Count, actualResponseData.Count);
  }

  [Fact]
  public async Task ProductsController_AddProduct_ShouldReturn201Status() {
    var expectedResponseData = ProductsMockData.AddProductResponse();
    var productService = new Mock<IProductService>();
    productService.Setup(static _ => _.AddProduct(It.IsAny<CreateProductDto>(), It.IsAny<ClaimsPrincipal>()))
      .ReturnsAsync(expectedResponseData);

    var sut = new ProductsController(productService.Object);
    var result = await sut.AddProduct(It.IsAny<CreateProductDto>());

    var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
    var actualResponseData = Assert.IsType<ProductResponseWithUser>(createdResult.Value);

    Assert.Equal(201, createdResult.StatusCode);
    Assert.Equal(expectedResponseData.Name, actualResponseData.Name);
    Assert.Equal(expectedResponseData.User.Name, actualResponseData.User.Name);
  }

  [Theory]
  [InlineData(400, typeof(BadRequestException), ProductServiceErrors.InvalidName)]
  [InlineData(401, typeof(UnauthorizedException), "")]
  public async Task ProductsController_AddProduct_ShouldReturnBadStatus(int statusCode, Type exceptionType, string message) {
    var expectedException = (ServiceException)Activator.CreateInstance(exceptionType, message)!;
    var productService = new Mock<IProductService>();
    productService.Setup(static _ => _.AddProduct(It.IsAny<CreateProductDto>(), It.IsAny<ClaimsPrincipal>()))
      .ThrowsAsync(expectedException);

    var sut = new ProductsController(productService.Object);
    var act = async () => await sut.AddProduct(It.IsAny<CreateProductDto>());

    var actualException = await Assert.ThrowsAsync(exceptionType, act);

    Assert.Equal(statusCode, ((dynamic)actualException).StatusCode);

    if (message is not null) {
      Assert.Equal(message, ((dynamic)actualException).StatusMessage);
    }
  }
}
