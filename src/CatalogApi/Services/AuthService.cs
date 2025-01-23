using CatalogApi.Dtos;
using CatalogApi.Persistence.Repositories;
using CatalogApi.Errors;
using CatalogApi.Helpers;
using System.Security.Claims;
namespace CatalogApi.Services;

public interface IAuthService {
  Task<AuthSignInResponseDto> SignIn(AuthSignInRequestDto auth);
  Task<UserResponse> Authentication(ClaimsPrincipal userClaim);
}

public class AuthService(IUserRepository userRepository, IAuthenticationHelper authenticationHelper) : IAuthService {
  private readonly IUserRepository _userRepository = userRepository;
  private readonly IAuthenticationHelper _authenticationHelper = authenticationHelper;

  public async Task<AuthSignInResponseDto> SignIn(AuthSignInRequestDto auth) {
    var user = await _userRepository.GetByEmail(auth.Email)
      ?? throw new NotFoundException(AuthServiceErrors.IncorrectLogin.Value);

    var math = user.Password == auth.Password;

    if (!math) {
      throw new NotFoundException(AuthServiceErrors.IncorrectLogin.Value);
    }

    var token = _authenticationHelper.AuthenticationToken(user);
    return new AuthSignInResponseDto(token);
  }

  public async Task<UserResponse> Authentication(ClaimsPrincipal userClaim) {
    var userId = AuthenticationHelper.GetUserClaimId(userClaim);
    var user = await _userRepository.GetById(userId)
      ?? throw new BadRequestException(AuthServiceErrors.InvalidToken.Value);
    return UserResponse.FromDomain(user);
  }
}

