using CatalogApi.Dtos;
using CatalogApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CatalogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService service) : ControllerBase {
  private readonly IUserService _service = service;

  [HttpGet]
  public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsers() {
    var users = await _service.GetUsers();
    return Ok(users);
  }

  [HttpGet("{id:long}")]
  public async Task<ActionResult<UserResponseWithProducts>> GetUser(long id) {
    var user = await _service.GetUser(id);
    return Ok(user);
  }

  [Authorize]
  [HttpGet("products")]
  public async Task<ActionResult<ProductResponse>> GetUserProducts() {
    var products = await _service.GetUserProducts(User);
    return Ok(products);
  }

  [HttpPost]
  public async Task<ActionResult<UserResponseWithProducts>> AddUser(CreateUserDto user) {
    var userCreated = await _service.AddUser(user);
    return CreatedAtAction(nameof(GetUser), new { userCreated.Id }, userCreated);
  }

  [Authorize]
  [HttpPut]
  public async Task<ActionResult<UserResponse>> UpdateUser(UpdateUserDto user) {
    var userUpdated = await _service.UpdateUser(user, User);
    return Ok(userUpdated);
  }

  [HttpDelete("{id:long}")]
  public async Task<IActionResult> DeleteUser(long id) {
    await _service.DeleteUser(id);
    return NoContent();
  }
}
