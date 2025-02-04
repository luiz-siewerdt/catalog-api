using CatalogApi.Domain.Entities;
using CatalogApi.Tests.MockData;
using CatalogApi.Tests.Utils;
using CatalogApi.Services;
using CatalogApi.Persistence.Repositories;
using CatalogApi.Dtos;
using CatalogApi.Errors;
using System.Linq.Expressions;
using Moq;

namespace CatalogApi.Tests.Services;

public class TestProductService {
  private readonly Mock<IProductRepository> productRepository;
  private readonly Mock<IUserRepository> userRepository;
  private readonly Mock<ICategoryRepository> categoryRepository;

  public TestProductService() {
    productRepository = new Mock<IProductRepository>();
    userRepository = new Mock<IUserRepository>();
    categoryRepository = new Mock<ICategoryRepository>();
  }

  [Fact]
  public async Task ProductService_GetProducts_ShouldReturnProductResponseWithUserList() {
    var expectedResponseData = ProductsMockData.GetProductsWithUserAndCategories();
    productRepository.Setup(static _ => _.GetAllWithUsers()).ReturnsAsync(ProductsMockData.GetAll());

    var sut = new ProductService(productRepository.Object, userRepository.Object, categoryRepository.Object);

    var actualResponseData = await sut.GetProducts();

    Assert.Equal(expectedResponseData.Count(), actualResponseData.Count());

    productRepository.Verify(static _ => _.GetAllWithUsers(), Times.Once());
  }

  [Fact]
  public async Task ProductService_GetProduct_ShouldReturnProductResponseWithUserAndCategories() {
    var expectedResponseData = ProductsMockData.GetProductWithUserAndCategories();
    productRepository.Setup(static _ => _.GetById(
      It.IsAny<long>(),
      It.IsAny<Expression<Func<ProductDomain, object>>>(),
      It.IsAny<Expression<Func<ProductDomain, object>>>()
    )).ReturnsAsync(ProductsMockData.GetById(1));

    var sut = new ProductService(productRepository.Object, userRepository.Object, categoryRepository.Object);
    var actualResponseData = await sut.GetProduct(1);

    Assert.NotNull(actualResponseData);
    Assert.Equal(expectedResponseData.Name, actualResponseData.Name);

    productRepository.Verify(static _ => _.GetById(
      It.Is<long>(static id => id == 1),
      It.IsAny<Expression<Func<ProductDomain, object>>>(),
      It.IsAny<Expression<Func<ProductDomain, object>>>()
    ), Times.Once());
  }

  [Fact]
  public async Task ProductService_GetProduct_ShouldThrowNotFoundException() {
    var expectedException = new NotFoundException(ProductServiceErrors.NotFound);
    productRepository.Setup(static _ => _.GetById(
      It.IsAny<long>(),
      It.IsAny<Expression<Func<ProductDomain, object>>>(),
      It.IsAny<Expression<Func<ProductDomain, object>>>()
    )).ThrowsAsync(expectedException);

    var sut = new ProductService(productRepository.Object, userRepository.Object, categoryRepository.Object);
    var actualException = await Assert.ThrowsAsync<NotFoundException>(async () => await sut.GetProduct(1));

    Assert.Equal(expectedException.StatusMessage, actualException.StatusMessage);

    productRepository.Verify(static _ => _.GetById(
      It.Is<long>(static id => id == 1),
      It.IsAny<Expression<Func<ProductDomain, object>>>(),
      It.IsAny<Expression<Func<ProductDomain, object>>>()
      ), Times.Once());
  }

  [Fact]
  public async Task ProductService_AddProduct_ShouldAddProductSuccessfully() {
    var createProduct = new CreateProductDto("product 1", 10, 0);
    var productUser = UsersMockData.GetById(1);
    userRepository.Setup(static _ => _.GetById(It.IsAny<long>())).ReturnsAsync(productUser);
    productRepository.Setup(static _ => _.Add(It.IsAny<ProductDomain>()))
      .Callback<ProductDomain>(static p => p.Id = 1);

    var sut = new ProductService(productRepository.Object, userRepository.Object, categoryRepository.Object);
    var actualResponseData = await sut.AddProduct(createProduct, GenerateClaimsPrincipal.Generate(1));

    Assert.Equal(1, actualResponseData.Id);
    Assert.Equal(createProduct.Name, actualResponseData.Name);
    Assert.Equal(createProduct.Price, actualResponseData.Price);
    Assert.Equal(createProduct.Discount, actualResponseData.Discount);
    Assert.Equal(productUser.Name, actualResponseData.User.Name);

    userRepository.Verify(_ => _.GetById(It.Is<long>(id => id == productUser.Id)), Times.Once());
    productRepository.Verify(_ => _.Add(It.Is<ProductDomain>(
      e => e.Name == createProduct.Name && e.Price == createProduct.Price
      && e.Discount == createProduct.Discount
    )), Times.Once());
  }

  [Fact]
  public async Task ProductService_AddProduct_ShouldThrowBadRequestException() {
    var expectedException = new BadRequestException(ProductServiceErrors.InvalidName);
    var createProduct = new CreateProductDto("", 10, 0);
    var productUser = UsersMockData.GetById(1);

    var sut = new ProductService(productRepository.Object, userRepository.Object, categoryRepository.Object);
    var actualException = await Assert.ThrowsAsync<BadRequestException>(async () =>
        await sut.AddProduct(createProduct, GenerateClaimsPrincipal.Generate(1)));

    Assert.Equal(expectedException.StatusMessage, actualException.StatusMessage);

    userRepository.Verify(_ => _.GetById(It.IsAny<long>()), Times.Never());
    productRepository.Verify(_ => _.Add(It.IsAny<ProductDomain>()), Times.Never());
  }

  [Fact]
  public async Task ProductService_AddProduct_ShouldThrowNotFoundExceptionForUser() {
    var expectedException = new NotFoundException(UserServiceErrors.NotFound);
    var createProduct = new CreateProductDto("product 1", 10, 0);
    var productUser = UsersMockData.GetById(1);
    userRepository.Setup(static _ => _.GetById(It.IsAny<long>())).ReturnsAsync((UserDomain?)null);

    var sut = new ProductService(productRepository.Object, userRepository.Object, categoryRepository.Object);
    var actualException = await Assert.ThrowsAsync<NotFoundException>(async () =>
        await sut.AddProduct(createProduct, GenerateClaimsPrincipal.Generate(1)));

    Assert.Equal(expectedException.StatusMessage, actualException.StatusMessage);

    userRepository.Verify(_ => _.GetById(It.Is<long>(id => id == 1)), Times.Once());
    productRepository.Verify(_ => _.Add(It.IsAny<ProductDomain>()), Times.Never());
  }

  [Fact]
  public async Task ProductService_UpdateProduct_ShouldUpdateSuccessfully() {
    var updateProduct = new UpdateProductDto("name", 10, 0);
    productRepository.Setup(static _ => _.GetById(It.IsAny<long>()))
      .ReturnsAsync(ProductsMockData.GetById(1));

    var sut = new ProductService(productRepository.Object, userRepository.Object, categoryRepository.Object);
    var actualResponseData = await sut.UpdateProduct(1, updateProduct, GenerateClaimsPrincipal.Generate(1));

    Assert.NotNull(actualResponseData);
    Assert.Equal(updateProduct.Name, actualResponseData.Name);
    Assert.Equal(updateProduct.Discount, actualResponseData.Discount);
    Assert.Equal(updateProduct.Price, actualResponseData.Price);

    productRepository.Verify(static _ => _.GetById(It.Is<long>(static id => id == 1)), Times.Once());
    productRepository.Verify(_ => _.Update(It.Is<ProductDomain>(
            p => p.Name == updateProduct.Name && p.Price == updateProduct.Price
              && p.Discount == updateProduct.Discount
            )), Times.Once());
  }

  [Fact]
  public async Task ProductService_UpdateProduct_ShouldThrowBadRequestException() {
    var updateProduct = new UpdateProductDto("", 10, 0);
    var expectedException = new BadRequestException(ProductServiceErrors.InvalidName);

    var sut = new ProductService(productRepository.Object, userRepository.Object, categoryRepository.Object);
    var actualException = await Assert.ThrowsAsync<BadRequestException>(async () =>
        await sut.UpdateProduct(1, updateProduct, GenerateClaimsPrincipal.Generate(1)));

    Assert.Equal(expectedException.StatusMessage, actualException.StatusMessage);

    productRepository.Verify(static _ => _.GetById(It.IsAny<long>()), Times.Never());
    productRepository.Verify(_ => _.Update(It.IsAny<ProductDomain>()), Times.Never());
  }

  [Fact]
  public async Task ProductService_UpdateProduct_ShouldThrowNotFoundException() {
    var updateProduct = new UpdateProductDto("name", 10, 0);
    var expectedException = new NotFoundException(ProductServiceErrors.NotFound);
    productRepository.Setup(static _ => _.GetById(It.IsAny<long>()))
      .ReturnsAsync((ProductDomain?)null);

    var sut = new ProductService(productRepository.Object, userRepository.Object, categoryRepository.Object);
    var actualException = await Assert.ThrowsAsync<NotFoundException>(async () =>
        await sut.UpdateProduct(1, updateProduct, GenerateClaimsPrincipal.Generate(1)));

    Assert.Equal(expectedException.StatusMessage, actualException.StatusMessage);

    productRepository.Verify(static _ => _.GetById(It.IsAny<long>()), Times.Once());
    productRepository.Verify(_ => _.Update(It.IsAny<ProductDomain>()), Times.Never());
  }

  [Fact]
  public async Task ProductService_UpdateProduct_ShouldThrowUnauthorizationException() {
    var updateProduct = new UpdateProductDto("name", 10, 0);
    var expectedException = new UnauthorizedException();
    productRepository.Setup(static _ => _.GetById(It.IsAny<long>()))
      .ReturnsAsync(ProductsMockData.GetById(1, userId: 2));

    var sut = new ProductService(productRepository.Object, userRepository.Object, categoryRepository.Object);
    var actualException = await Assert.ThrowsAsync<UnauthorizedException>(async () =>
        await sut.UpdateProduct(1, updateProduct, GenerateClaimsPrincipal.Generate(1)));

    Assert.Equal(expectedException.StatusMessage, actualException.StatusMessage);

    productRepository.Verify(static _ => _.GetById(It.IsAny<long>()), Times.Once());
    productRepository.Verify(_ => _.Update(It.IsAny<ProductDomain>()), Times.Never());
  }

  // TODO: test DeleteProduct, GetProductsByCategories and AddProductCategory methods

  [Fact]
  public async Task ProductService_AddProductCategory_ShouldAddCategorySuccessfully() {
    var product = ProductsMockData.GetById();
    var categoryList = CategoriesMockData.GetAll().ToArray();
    var categoryNames = categoryList.Select(e => e.Name).ToArray();
    productRepository.Setup(static _ => _.GetByIdWithCategories(It.IsAny<long>())).ReturnsAsync(product);

    categoryRepository.Setup(_ => _.GetByName(It.Is<string>(c => c == categoryList[0].Name)))
      .ReturnsAsync(categoryList[0]);

    categoryRepository.Setup(_ => _.GetByName(It.Is<string>(c => c == categoryList[1].Name)))
      .ReturnsAsync(categoryList[1]);

    categoryRepository.Setup(_ => _.GetByName(It.Is<string>(c => c == categoryList[2].Name)))
      .ReturnsAsync(categoryList[2]);

    var sut = new ProductService(productRepository.Object, userRepository.Object, categoryRepository.Object);
    await sut.AddProductCategory(categoryNames, 1, GenerateClaimsPrincipal.Generate(1));

    var productCategories = product.Categories.ToList();

    Assert.Equal(categoryNames.Length, product.Categories.Count);

    for (var i = 0; i < product.Categories.Count; ++i) {
      Assert.Equal(categoryNames[i], productCategories[i].Name);
    }

    categoryRepository.Verify(static _ => _.GetByName(It.IsAny<string>()), Times.Exactly(3));
    productRepository.Verify(static _ => _.GetByIdWithCategories(It.Is<long>(id => id == 1)), Times.Once());
    productRepository.Verify(static _ => _.Update(It.IsAny<ProductDomain>()), Times.Once());
  }

  [Fact]
  public async Task ProductService_AddProductCategory_ShouldThrowProductNotFoundException() {
    var expectedException = new NotFoundException(ProductServiceErrors.NotFound);
    productRepository.Setup(static _ => _.GetByIdWithCategories(It.IsAny<long>())).ThrowsAsync(expectedException);

    var sut = new ProductService(productRepository.Object, userRepository.Object, categoryRepository.Object);
    var actualException = await Assert.ThrowsAsync<NotFoundException>(
        async () => await sut.AddProductCategory([], 1, GenerateClaimsPrincipal.Generate(1)));

    Assert.Equal(expectedException.StatusMessage, actualException.StatusMessage);

    productRepository.Verify(static _ => _.GetByIdWithCategories(It.Is<long>(id => id == 1)), Times.Once());
    productRepository.Verify(static _ => _.Update(It.IsAny<ProductDomain>()), Times.Never());
    categoryRepository.Verify(static _ => _.GetByName(It.IsAny<string>()), Times.Never());
  }

  [Fact]
  public async Task ProductService_AddProductCategory_ShouldThrowUnauthorizedException() {
    var product = ProductsMockData.GetById(userId: 2);
    var expectedException = new UnauthorizedException();
    productRepository.Setup(static _ => _.GetByIdWithCategories(It.IsAny<long>())).ReturnsAsync(product);

    var sut = new ProductService(productRepository.Object, userRepository.Object, categoryRepository.Object);
    var actualException = await Assert.ThrowsAsync<UnauthorizedException>(
        async () => await sut.AddProductCategory([], 1, GenerateClaimsPrincipal.Generate(1)));

    productRepository.Verify(static _ => _.GetByIdWithCategories(It.Is<long>(id => id == 1)), Times.Once());
    productRepository.Verify(static _ => _.Update(It.IsAny<ProductDomain>()), Times.Never());
    categoryRepository.Verify(static _ => _.GetByName(It.IsAny<string>()), Times.Never());
  }

  [Fact]
  public async Task ProductService_AddProductCategory_ShouldThrowCategoryNotFound() {
    var product = ProductsMockData.GetById(userId: 2);
    var expectedException = new UnauthorizedException();
    productRepository.Setup(static _ => _.GetByIdWithCategories(It.IsAny<long>())).ReturnsAsync(product);

    var sut = new ProductService(productRepository.Object, userRepository.Object, categoryRepository.Object);
    var actualException = await Assert.ThrowsAsync<UnauthorizedException>(
        async () => await sut.AddProductCategory([], 1, GenerateClaimsPrincipal.Generate(1)));

    productRepository.Verify(static _ => _.GetByIdWithCategories(It.Is<long>(id => id == 1)), Times.Once());
    productRepository.Verify(static _ => _.Update(It.IsAny<ProductDomain>()), Times.Never());
    categoryRepository.Verify(static _ => _.GetByName(It.IsAny<string>()), Times.Never());

  }

}
