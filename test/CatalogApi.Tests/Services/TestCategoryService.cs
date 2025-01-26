using CatalogApi.Domain.Entities;
using CatalogApi.Dtos;
using CatalogApi.Errors;
using CatalogApi.Persistence.Repositories;
using CatalogApi.Services;
using Moq;
using CatalogApi.Tests.MockData;

namespace CatalogApi.Tests.Services;

public class TestCategoryService {
  [Fact]
  public async Task CategoryService_GetCategories_ShouldReturnCategoryResponseList() {
    var expectedResponseData = CategoriesMockData.GetCategories();
    var categoryRepository = new Mock<ICategoryRepository>();
    categoryRepository.Setup(static _ => _.GetAll()).ReturnsAsync(CategoriesMockData.GetAll());

    var sut = new CategoryService(categoryRepository.Object);
    var result = await sut.GetCategories();

    Assert.Equal(CategoriesMockData.GetAll().First().Name, result.First().Name);
  }

  [Fact]
  public async Task CategoryService_GetCategory_ShouldReturnCategoryResponse() {
    var categoryRepository = new Mock<ICategoryRepository>();
    categoryRepository.Setup(static _ => _.GetById(It.IsAny<long>()))
      .ReturnsAsync(CategoriesMockData.GetById());

    var sut = new CategoryService(categoryRepository.Object);
    var result = await sut.GetCategory(It.IsAny<long>());

    Assert.Equal("category 1", result.Name);
  }

  [Fact]
  public async Task CategoryService_GetCategory_ShouldThrowNotFound() {
    var expectedException = new NotFoundException(CategoryServiceErrors.NotFound);
    var categoryRepository = new Mock<ICategoryRepository>();
    categoryRepository.Setup(static _ => _.GetById(It.IsAny<long>()))
      .ThrowsAsync(expectedException);

    var sut = new CategoryService(categoryRepository.Object);
    var act = async () => await sut.GetCategory(It.IsAny<long>());

    var actualException = await Assert.ThrowsAsync<NotFoundException>(act);

    Assert.Equal(404, actualException.StatusCode);
    Assert.Equal(expectedException.StatusMessage, actualException.StatusMessage);
  }

  [Fact]
  public async Task CategoryService_AddCategory_ShouldCreateSuccessfully() {
    var createCategoryDto = new CreateCategoryDto("category");
    var categoryRepository = new Mock<ICategoryRepository>();
    categoryRepository.Setup(static _ => _.Add(It.IsAny<CategoryDomain>()))
      .Callback<CategoryDomain>(static e => e.Id = 1);


    var sut = new CategoryService(categoryRepository.Object);

    var result = await sut.AddCategory(createCategoryDto);

    Assert.NotNull(result);
    Assert.Equal(1, result.Id);
    Assert.Equal(createCategoryDto.Name, result.Name);

    categoryRepository.Verify(repo => repo.Add(It.Is<CategoryDomain>(
      category => category.Name == createCategoryDto.Name
    )), Times.Once);
    categoryRepository.Verify(repo => repo.NameAlreadyInUse(
        It.Is<string>(s => s.Equals(createCategoryDto.Name)),
        It.Is<long>(static id => id == 0)),
      Times.Once);
  }

  [Fact]
  public async Task CategoryService_AddCategory_ShouldThrowBadRequestExceptionForInvalidName() {
    var createCategoryDto = new CreateCategoryDto(null);
    var expectedException = new BadRequestException(CategoryServiceErrors.InvalidName);
    var categoryRepository = new Mock<ICategoryRepository>();

    var sut = new CategoryService(categoryRepository.Object);

    var actualException = await Assert.ThrowsAsync<BadRequestException>(async () => await sut.AddCategory(createCategoryDto));
    Assert.NotNull(actualException);
    Assert.Equal(400, actualException.StatusCode);
    Assert.Equal(expectedException.StatusMessage, actualException.StatusMessage);

    categoryRepository.Verify(repo => repo.Add(It.IsAny<CategoryDomain>()), Times.Never());
    categoryRepository.Verify(repo => repo.NameAlreadyInUse(
        It.Is<string>(s => s == createCategoryDto.Name),
        It.Is<long>(static id => id == 0)),
      Times.Once);
  }

  [Fact]
  public async Task CategoryService_AddCategory_ShouldThrowBadRequestExceptionForNameAlreadyExists() {
    var createCategoryDto = new CreateCategoryDto("name");
    var expectedException = new BadRequestException(CategoryServiceErrors.NameAlreadyExists);
    var categoryRepository = new Mock<ICategoryRepository>();
    categoryRepository.Setup(static _ => _.NameAlreadyInUse(It.IsAny<string>(), It.IsAny<long>()))
      .ReturnsAsync(true);

    var sut = new CategoryService(categoryRepository.Object);

    var actualException = await Assert.ThrowsAsync<BadRequestException>(async () => await sut.AddCategory(createCategoryDto));
    Assert.NotNull(actualException);
    Assert.Equal(400, actualException.StatusCode);
    Assert.Equal(expectedException.StatusMessage, actualException.StatusMessage);

    categoryRepository.Verify(repo => repo.Add(It.IsAny<CategoryDomain>()), Times.Never());
    categoryRepository.Verify(repo => repo.Add(It.IsAny<CategoryDomain>()), Times.Never());
    categoryRepository.Verify(repo => repo.NameAlreadyInUse(
        It.Is<string>(s => s.Equals(createCategoryDto.Name)),
        It.Is<long>(static id => id == 0)),
      Times.Once);
  }

  [Fact]
  public async Task CategoryService_UpdateCategory_ShouldUpdateSuccessfully() {
    var updateCategory = new UpdateCategoryDto("name");
    long categoryId = 5;

    var categoryRepository = new Mock<ICategoryRepository>();
    categoryRepository.Setup(static _ => _.GetById(It.IsAny<long>())).ReturnsAsync(
      new CategoryDomain { Id = categoryId, Name = "na", Products = [] });
    categoryRepository.Setup(static _ => _.Update(It.IsAny<CategoryDomain>()))
      .Callback<CategoryDomain>(static cat => cat.Name = "name");

    var sut = new CategoryService(categoryRepository.Object);

    var result = await sut.UpdateCategory(categoryId, updateCategory);

    Assert.NotNull(result);
    Assert.Equal(categoryId, result.Id);
    Assert.Equal(updateCategory.Name, result.Name);

    categoryRepository.Verify(repo => repo.Update(It.Is<CategoryDomain>(
      category => category.Name == updateCategory.Name
        && category.Id == categoryId
    )), Times.Once);
    categoryRepository.Verify(repo => repo.NameAlreadyInUse(
        It.Is<string>(s => s.Equals(updateCategory.Name)),
        It.Is<long>(id => id == categoryId)),
      Times.Once);
  }

  //  TODO: Add UpdateCategory and DeleteCategory tests

}
