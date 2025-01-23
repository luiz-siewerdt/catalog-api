using Microsoft.AspNetCore.Mvc;
using CatalogApi.Services;
using CatalogApi.Dtos;
using Microsoft.AspNetCore.Authorization;
namespace CatalogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService service) : ControllerBase {
  private readonly IAuthService _service = service;

  [HttpPost("signIn")]
  public async Task<ActionResult<AuthSignInResponseDto>> SignIn(AuthSignInRequestDto body) {
    var token = await _service.SignIn(body);
    return Ok(token);
  }

  [Authorize]
  [HttpPatch("authentication")]
  public async Task<IActionResult> Authentication() {
    var user = await _service.Authentication(User);
    return Ok(user);
  }
}
