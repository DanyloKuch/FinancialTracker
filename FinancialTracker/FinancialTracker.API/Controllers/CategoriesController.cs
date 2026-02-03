using FinancialTracker.Application.DTOs;
using FinancialTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancialTracker.API.Controllers
{
    [ApiController]
    [Route("api/v1/categories")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetUserCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(Guid id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(new { message = result.Error });

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryRequest request)
        {
            var result = await _categoryService.CreateCategoryAsync(request);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Error });

            return Ok(new { id = result.Value });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] CategoryRequest request)
        {
            var result = await _categoryService.UpdateCategoryAsync(id, request);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Error });

            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);

            if (!result.IsSuccess)
                return NotFound(new { message = result.Error });

            return NoContent();
        }

        [HttpGet("{id}/total")]
        public async Task<IActionResult> GetCategoryTotal(Guid id)
        {
            var result = await _categoryService.GetCategoryTotalAmountAsync(id);

            if (!result.IsSuccess)
                return NotFound(new { message = result.Error });

            return Ok(new { CategoryId = id, TotalAmount = result.Value });
        }
    }
}