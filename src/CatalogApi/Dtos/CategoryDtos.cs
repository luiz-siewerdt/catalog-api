using CatalogApi.Domain.Entities;

namespace CatalogApi.Dtos;

public record CategoryDto(string Name, long? Id = 0);

public record CreateCategoryDto(string Name)
  : CategoryDto(Name);

public record UpdateCategoryDto(string Name)
  : CategoryDto(Name);


public record CategoryResponse(long Id, string Name) {
  public static CategoryResponse FromDomain(CategoryDomain categoryDomain) {
    return new CategoryResponse(categoryDomain.Id, categoryDomain.Name);
  }
}

public static class CategoryDtoExtension {
  public static CategoryDomain ToDomain(this CategoryDto categoryDto) {
    return new CategoryDomain {
      Id = categoryDto.Id ?? 0,
      Name = categoryDto.Name,
      Products = []
    };
  }
}


