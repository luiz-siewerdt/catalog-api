using CatalogApi.Dtos;
using FluentValidation;
using CatalogApi.Errors;
namespace CatalogApi.Helpers.Validators;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto> {
  public CreateProductDtoValidator() {
    RuleFor(static product => product.Name).NotEmpty().WithMessage(ProductServiceErrors.InvalidName);
    RuleFor(static product => product.Price).GreaterThanOrEqualTo(0).WithMessage(ProductServiceErrors.InvalidPrice);
    RuleFor(static product => product.Discount).GreaterThanOrEqualTo(0).WithMessage(ProductServiceErrors.InvalidDiscount);
  }
}

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto> {
  public UpdateProductDtoValidator() {
    RuleFor(static product => product.Name).NotEmpty().WithMessage(ProductServiceErrors.InvalidName);
    RuleFor(static product => product.Price).GreaterThanOrEqualTo(0).WithMessage(ProductServiceErrors.InvalidPrice);
    RuleFor(static product => product.Discount).GreaterThanOrEqualTo(0).WithMessage(ProductServiceErrors.InvalidDiscount);
  }
}
