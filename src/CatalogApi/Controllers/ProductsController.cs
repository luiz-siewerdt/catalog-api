using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CatalogApi.Services;
using CatalogApi.Dtos;

namespace CatalogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductService service) : ControllerBase {
  private readonly IProductService _service = service;

  [HttpGet]
  public async Task<ActionResult<IEnumerable<ProductResponseWithUser>>> GetProducts() {
    var products = await _service.GetProducts();
    return Ok(products);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<ProductResponseWithUserAndCategories>> GetProduct(long id) {
    var product = await _service.GetProduct(id);
    return Ok(product);
  }

  [HttpGet("filters/by-category")]
  public async Task<ActionResult<ProductResponseWithUser>> GetProductsByCategory(List<string> categoryNames) {
    var product = await _service.GetProductsByCategories(categoryNames);
    return Ok(product);
  }

  [Authorize]
  [HttpPost]
  public async Task<ActionResult<ProductResponseWithUser>> AddProduct(CreateProductDto product) {
    var createdProduct = await _service.AddProduct(product, User);
    return CreatedAtAction(nameof(GetProduct), new { createdProduct.Id }, createdProduct);
  }

  [Authorize]
  [HttpPost("{productId:long}/categories")]
  public async Task<IActionResult> AddProductCategories(long productId, ICollection<string> categoryNames) {
    await _service.AddProductCategory(categoryNames, productId, User);
    return Ok();
  }

  [Authorize]
  [HttpPut("{id}")]
  public async Task<ActionResult<ProductResponse>> UpdateProduct(long id, UpdateProductDto product) {
    var updatedProduct = await _service.UpdateProduct(id, product, User);
    return Ok(updatedProduct);
  }

  [Authorize]
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteProduct(long id) {
    await _service.DeleteProduct(id, User);
    return NoContent();
  }

}
