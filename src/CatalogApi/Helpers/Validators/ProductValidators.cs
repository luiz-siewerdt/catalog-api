using CatalogApi.Dtos;
using FluentValidation;
using CatalogApi.Errors;
namespace CatalogApi.Helpers.Validators;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto> {
  public CreateProductDtoValidator() {
    RuleFor(static product => product.Name).NotNull().WithMessage(ProductServiceErrors.InvalidName.Value);
    RuleFor(static product => product.Price).GreaterThanOrEqualTo(0).WithMessage(ProductServiceErrors.InvalidPrice.Value);
    RuleFor(static product => product.Discount).GreaterThanOrEqualTo(0).WithMessage(ProductServiceErrors.InvalidDiscount.Value);
  }
}

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto> {
  public UpdateProductDtoValidator() {
    RuleFor(static product => product.Name).NotNull().WithMessage(ProductServiceErrors.InvalidName.Value);
    RuleFor(static product => product.Price).GreaterThanOrEqualTo(0).WithMessage(ProductServiceErrors.InvalidPrice.Value);
    RuleFor(static product => product.Discount).GreaterThanOrEqualTo(0).WithMessage(ProductServiceErrors.InvalidDiscount.Value);
  }
}
