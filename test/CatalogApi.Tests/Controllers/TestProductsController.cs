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
    var expectedResponseData = ProductsMockData.GetProductsWithUser();
    var productService = new Mock<IProductService>();
    productService.Setup(static _ => _.GetProducts()).ReturnsAsync(expectedResponseData);

    var sut = new ProductsController(productService.Object);
    var result = await sut.GetProducts();

    var okResult = Assert.IsType<OkObjectResult>(result.Result);
    var actualResponseData = Assert.IsAssignableFrom<IEnumerable<ProductResponseWithUser>>(okResult.Value);

    Assert.Equal(200, okResult.StatusCode);
    Assert.Equal(expectedResponseData.Count(), actualResponseData.Count());
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

  [Fact]
  public async Task ProductsController_UpdateProduct_ShouldReturn200Status() {
    var expectedResponseData = new ProductResponse(1, "product", 10, 0, 1);
    var productService = new Mock<IProductService>();
    productService.Setup(static _ => _.UpdateProduct(It.IsAny<long>(), It.IsAny<UpdateProductDto>(), It.IsAny<ClaimsPrincipal>()))
      .ReturnsAsync(expectedResponseData);

    var sut = new ProductsController(productService.Object);
    var result = await sut.UpdateProduct(1, It.IsAny<UpdateProductDto>());

    var okObject = Assert.IsType<OkObjectResult>(result.Result);
    var actualResponseData = Assert.IsType<ProductResponse>(okObject.Value);

    Assert.Equal(expectedResponseData.Name, actualResponseData.Name);
    Assert.Equal(expectedResponseData.Price, actualResponseData.Price);
    Assert.Equal(expectedResponseData.Discount, actualResponseData.Discount);
    Assert.Equal(expectedResponseData.UserId, actualResponseData.UserId);
  }

  [Theory]
  [InlineData(400, typeof(BadRequestException), ProductServiceErrors.InvalidName)]
  [InlineData(401, typeof(UnauthorizedException), "")]
  [InlineData(404, typeof(NotFoundException), ProductServiceErrors.NotFound)]
  public async Task ProductsController_UpdateProduct_ShouldReturnBadStatus(int statusCode, Type exceptionType, string message) {
    var expectedException = (ServiceException)Activator.CreateInstance(exceptionType, message)!;
    var productService = new Mock<IProductService>();
    productService.Setup(static _ => _.UpdateProduct(It.IsAny<long>(), It.IsAny<UpdateProductDto>(), It.IsAny<ClaimsPrincipal>()))
      .ThrowsAsync(expectedException);

    var sut = new ProductsController(productService.Object);
    var actualException = await Assert.ThrowsAsync(exceptionType,
        async () => await sut.UpdateProduct(It.IsAny<long>(), It.IsAny<UpdateProductDto>()));

    Assert.Equal(statusCode, ((dynamic)actualException).StatusCode);
    if (message != "") {
      Assert.Equal(message, ((dynamic)actualException).StatusMessage);
    }
  }

  [Fact]
  public async Task ProductsController_DeleteProduct_ShouldReturn204Status() {
    var productService = new Mock<IProductService>();
    var sut = new ProductsController(productService.Object);

    var result = await sut.DeleteProduct(It.IsAny<long>());

    var noContentResult = Assert.IsType<NoContentResult>(result);

    Assert.Equal(204, noContentResult.StatusCode);
  }

  [Fact]
  public async Task ProductsController_DeleteProduct_ShouldReturn404Status() {
    var expectedException = new NotFoundException(ProductServiceErrors.NotFound);
    var productService = new Mock<IProductService>();
    productService.Setup(static _ => _.DeleteProduct(It.IsAny<long>(), It.IsAny<ClaimsPrincipal>()))
      .ThrowsAsync(expectedException);

    var sut = new ProductsController(productService.Object);

    var exception = await Assert.ThrowsAsync<NotFoundException>(async () => await sut.DeleteProduct(It.IsAny<long>()));
    Assert.Equal(ProductServiceErrors.NotFound, exception.StatusMessage);
  }
}
