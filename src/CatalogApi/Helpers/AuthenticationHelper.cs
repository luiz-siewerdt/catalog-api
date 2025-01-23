using CatalogApi.Domain.Entities;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using CatalogApi.Errors;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Globalization;
namespace CatalogApi.Helpers;

public interface IAuthenticationHelper {
  string AuthenticationToken(UserDomain user);
}

public class AuthenticationHelper(IConfiguration configuration) : IAuthenticationHelper {
  private readonly IConfiguration _configuration = configuration;

  public string AuthenticationToken(UserDomain user) {
    var claims = new List<Claim> {
      new(ClaimTypes.NameIdentifier, user.Id.ToString()),
      new(ClaimTypes.Name, user.Name)
    };

    var jwtToken = new JwtSecurityToken(
        claims: claims,
        notBefore: DateTime.UtcNow,
        expires: DateTime.UtcNow.AddDays(1),
        signingCredentials: new SigningCredentials(
          new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["ProjectSettings:JWT_Secret"] ?? "token")), SecurityAlgorithms.HmacSha256));
    var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
    Console.WriteLine(token);
    return token;
  }

  public static long GetUserClaimId(ClaimsPrincipal principal) {
    var strId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
      ?? throw new NotFoundException("osfdjskjfl");

    return long.Parse(strId, CultureInfo.InvariantCulture);
  }
}
