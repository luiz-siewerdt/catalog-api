using Microsoft.AspNetCore.Mvc;
using CatalogApi.Dtos;
using CatalogApi.Services;

namespace CatalogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ICategoryService service) : ControllerBase {
  private readonly ICategoryService _service = service;

  [HttpGet]
  public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetCategories() {
    var categories = await _service.GetCategories();
    return Ok(categories);
  }

  [HttpGet("{id:long}")]
  public async Task<ActionResult<CategoryResponse>> GetCategory(long id) {
    var category = await _service.GetCategory(id);
    return Ok(category);
  }

  [HttpPost]
  public async Task<ActionResult<CategoryResponse>> AddCategory(CreateCategoryDto category) {
    var categoryResponse = await _service.AddCategory(category);
    return CreatedAtAction(nameof(GetCategory), new { categoryResponse.Id }, categoryResponse);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<CategoryResponse>> UpdateCategory(long id, UpdateCategoryDto category) {
    var categoryResponse = await _service.UpdateCategory(id, category);
    return Ok(categoryResponse);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult> DeleteCategory(long id) {
    await _service.DeleteCategory(id);
    return NoContent();
  }
}
