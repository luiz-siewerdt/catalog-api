using CatalogApi.Dtos;
using FluentValidation;
using CatalogApi.Errors;
using CatalogApi.Persistence.Repositories;

namespace CatalogApi.Helpers.Validators;

public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto> {
  public CreateCategoryDtoValidator(ICategoryRepository repository) {
    RuleFor(static e => e.Name).NotEmpty().WithMessage(CategoryServiceErrors.InvalidName)
      .CustomAsync(async (name, ctx, CancellationToken) => {
        var nameAlreadyInUse = await repository.NameAlreadyInUse(name);
        if (nameAlreadyInUse) {
          ctx.AddFailure(CategoryServiceErrors.NameAlreadyExists);
        }
      });
  }
}

public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto> {
  public UpdateCategoryDtoValidator(ICategoryRepository repository, long id) {
    RuleFor(static e => e.Name).NotEmpty().WithMessage(CategoryServiceErrors.InvalidName)
      .CustomAsync(async (name, ctx, cancellationToken) => {
        var nameAlreadyInUse = await repository.NameAlreadyInUse(name, id);
        if (nameAlreadyInUse) {
          ctx.AddFailure(CategoryServiceErrors.NameAlreadyExists);
        }
      });
  }
}
