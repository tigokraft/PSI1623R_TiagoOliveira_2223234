using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinSync.Data;
using FinSync.DTOs;
using FinSync.Models;

namespace FinSync.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BudgetController : ControllerBase
    {
        private readonly FinSyncContext _context;

        public BudgetController(FinSyncContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddBudget([FromBody] CreateBudgetDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid token.");

            var budget = new Budget
            {
                UserId = userId.Value,
                CategoryId = dto.CategoryId,
                MonthlyLimit = dto.MonthlyLimit
            };
            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Budget created." });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BudgetDto>>> GetBudgets()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid token.");

            var budgets = await _context.Budgets
                .Where(b => b.UserId == userId.Value)
                .Select(b => new BudgetDto
                {
                    BudgetId = b.BudgetId,
                    MonthlyLimit = b.MonthlyLimit,
                    CategoryId = b.CategoryId
                })
                .ToListAsync();

            return Ok(budgets);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBudget(int id, [FromBody] CreateBudgetDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid token.");

            var budget = await _context.Budgets.FirstOrDefaultAsync(b => b.BudgetId == id && b.UserId == userId.Value);
            if (budget == null) return NotFound();

            budget.CategoryId = dto.CategoryId;
            budget.MonthlyLimit = dto.MonthlyLimit;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Budget updated." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBudget(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid token.");

            var budget = await _context.Budgets.FirstOrDefaultAsync(b => b.BudgetId == id && b.UserId == userId.Value);
            if (budget == null) return NotFound();

            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Budget removed." });
        }

        private int? GetUserId()
        {
            var claim = User.FindFirst("userId")?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }
    }
}