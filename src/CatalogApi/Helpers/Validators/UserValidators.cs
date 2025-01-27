using CatalogApi.Dtos;
using FluentValidation;
using CatalogApi.Errors;
using CatalogApi.Persistence.Repositories;

namespace CatalogApi.Helpers.Validators;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto> {
  public CreateUserDtoValidator(IUserRepository repository) {
    RuleFor(static user => user.Name).NotEmpty().WithMessage(UserServiceErrors.InvalidName);
    RuleFor(static user => user.Email).EmailAddress().WithMessage(UserServiceErrors.InvalidEmail);
    RuleFor(static user => user.Email).CustomAsync(async (email, ctx, cancellationToken) => {
      var emailExists = await repository.EmailAlredyInUse(email);
      if (emailExists) {
        ctx.AddFailure(UserServiceErrors.EmailAlreadyInUse);
      }
    });
    RuleFor(static user => user.Password).NotEmpty().WithMessage(UserServiceErrors.InvalidPassword);
    RuleFor(static user => user.ConfPassword).Equal(static user => user.Password).WithMessage(UserServiceErrors.NotIqualPassword);
  }
}

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto> {
  public UpdateUserDtoValidator(IUserRepository repository, long userId) {
    RuleFor(static user => user.Name).NotEmpty().WithMessage(UserServiceErrors.InvalidName);
    RuleFor(static user => user.Email).EmailAddress().WithMessage(UserServiceErrors.InvalidEmail);
    RuleFor(static user => user.Email).CustomAsync(async (email, ctx, cancellationToken) => {
      var emailExists = await repository.EmailAlredyInUse(email, userId);
      if (emailExists) {
        ctx.AddFailure(UserServiceErrors.EmailAlreadyInUse);
      }
    });
  }
}
