using System.Security.Claims;

namespace CatalogApi.Tests.Utils;
public class GenerateClaimsPrincipal {
  public static ClaimsPrincipal Generate(long id = 1) {
    var claimList = new List<Claim> { new(ClaimTypes.NameIdentifier, id.ToString()) };
    var claimsPrincipal = new ClaimsPrincipal();
    claimsPrincipal.AddIdentity(new(claimList));
    return claimsPrincipal;
  }
}
