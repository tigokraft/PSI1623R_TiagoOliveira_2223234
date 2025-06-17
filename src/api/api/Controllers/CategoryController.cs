using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using FinSync.Data;
using FinSync.DTOs;
using FinSync.Models;

namespace FinSync.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly FinSyncContext _context;

        public CategoryController(FinSyncContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid token.");

            var categories = await _context.Categories
                .Where(c => c.UserId == userId.Value)
                .OrderBy(c => c.CategoryName)
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName
                })
                .ToListAsync();
            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid token.");

            if (string.IsNullOrWhiteSpace(dto.CategoryName))
                return BadRequest("Category name required.");

            if (await _context.Categories.AnyAsync(c => c.UserId == userId.Value && c.CategoryName == dto.CategoryName))
                return Conflict("Category already exists.");

            var category = new Category { CategoryName = dto.CategoryName, UserId = userId.Value };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            dto.CategoryId = category.CategoryId;
            return Ok(dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid token.");

            var cat = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id && c.UserId == userId.Value);
            if (cat == null) return NotFound();

            if (string.IsNullOrWhiteSpace(dto.CategoryName))
                return BadRequest("Category name required.");

            cat.CategoryName = dto.CategoryName;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Category updated." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid token.");

            var cat = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id && c.UserId == userId.Value);
            if (cat == null) return NotFound();

            _context.Categories.Remove(cat);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Category removed." });
        }

        private int? GetUserId()
        {
            var claim = User.FindFirst("userId")?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }
    }
}