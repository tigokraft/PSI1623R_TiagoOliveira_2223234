using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinSync.Data;
using FinSync.DTOs;
using FinSync.Models;
using System.Security.Claims;

namespace FinSync.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExpenseController : ControllerBase
    {
        private readonly FinSyncContext _context;

        public ExpenseController(FinSyncContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddExpense([FromBody] CreateExpenseDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid token.");

            var expense = new Expense
            {
                UserId = userId.Value,
                Amount = dto.Amount,
                Tags = dto.Tags,
                Description = dto.Description,
                Date = dto.Date,
                CategoryId = dto.CategoryId
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Expense added." });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpenseDto>>> GetExpenses()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid token.");

            var expenses = await _context.Expenses
                .Include(e => e.Category)
                .Where(e => e.UserId == userId.Value)
                .OrderByDescending(e => e.Date)
                .Select(e => new ExpenseDto
                {
                    ExpenseId = e.ExpenseId,
                    Amount = e.Amount,
                    Tags = e.Tags,
                    Description = e.Description,
                    Date = e.Date,
                    CategoryName = e.Category.CategoryName
                })
                .ToListAsync();

            return Ok(expenses);
        }

        [HttpGet("summary")]
        [Authorize]
        public IActionResult GetExpenseSummary()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Invalid user token.");
        
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
        
            var monthlyTotal = _context.Expenses
                .Where(e => e.UserId == userId && e.Date >= startOfMonth)
                .Sum(e => (decimal?)e.Amount) ?? 0;
        
            var allTimeTotal = _context.Expenses
                .Where(e => e.UserId == userId)
                .Sum(e => (decimal?)e.Amount) ?? 0;
        
            var expenses = _context.Expenses
                .Where(e => e.UserId == userId)
                .Select(e => new ExpenseItemDto
                {
                    ExpenseId = e.ExpenseId,
                    Amount = e.Amount,
                    Description = e.Description,
                    Date = e.Date,
                    Tags = e.Tags,
                    CategoryName = e.Category != null ? e.Category.CategoryName : null
                })
                .ToList();
        
            var result = new MonthlyExpenseSummaryDto
            {
                TotalMonthlySpent = monthlyTotal,
                TotalAllTimeSpent = allTimeTotal,
                Expenses = expenses
            };
        
            return Ok(result);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid token.");

            var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.ExpenseId == id && e.UserId == userId.Value);
            if (expense == null)
                return NotFound("Expense not found.");

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Expense deleted." });
        }

        private int? GetUserId()
        {
            var claim = User.FindFirst("userId")?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }
    }
}
