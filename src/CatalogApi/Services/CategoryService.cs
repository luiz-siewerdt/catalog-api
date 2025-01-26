using CatalogApi.Dtos;
using CatalogApi.Errors;
using CatalogApi.Helpers.Validators;
using CatalogApi.Persistence.Repositories;

namespace CatalogApi.Services;

public interface ICategoryService {
  Task<IEnumerable<CategoryResponse>> GetCategories();
  Task<CategoryResponse> GetCategory(long id);
  Task<CategoryResponse> AddCategory(CreateCategoryDto category);
  Task<CategoryResponse> UpdateCategory(long id, UpdateCategoryDto category);
  Task DeleteCategory(long id);
}

public class CategoryService(ICategoryRepository repository) : ICategoryService {
  private readonly ICategoryRepository _repository = repository;

  public async Task<IEnumerable<CategoryResponse>> GetCategories() {
    var categories = await _repository.GetAll();
    return categories.Select(CategoryResponse.FromDomain);
  }

  public async Task<CategoryResponse> GetCategory(long id) {
    var category = await _repository.GetById(id)
      ?? throw new NotFoundException(CategoryServiceErrors.NotFound);

    return CategoryResponse.FromDomain(category);
  }

  public async Task<CategoryResponse> AddCategory(CreateCategoryDto category) {
    var categoryValidator = new CreateCategoryDtoValidator(_repository);
    var validator = await categoryValidator.ValidateAsync(category);

    if (!validator.IsValid) {
      throw new BadRequestException(validator.Errors.First().ErrorMessage);
    }

    var categoryDomain = category.ToDomain();
    await _repository.Add(categoryDomain);
    return CategoryResponse.FromDomain(categoryDomain);
  }

  public async Task<CategoryResponse> UpdateCategory(long id, UpdateCategoryDto category) {

    var categoryValidator = new UpdateCategoryDtoValidator(_repository, id);
    var validator = await categoryValidator.ValidateAsync(category);

    if (!validator.IsValid) {
      throw new BadRequestException(validator.Errors.First().ErrorMessage);
    }

    var categoryDomain = await _repository.GetById(id)
      ?? throw new NotFoundException(CategoryServiceErrors.NotFound);

    categoryDomain.Name = category.Name;

    await _repository.Update(categoryDomain);
    return CategoryResponse.FromDomain(categoryDomain);
  }

  public async Task DeleteCategory(long id) {
    var categoryExists = await _repository.GetById(id)
      ?? throw new NotFoundException(CategoryServiceErrors.NotFound);

    await _repository.Remove(categoryExists);
  }
}

