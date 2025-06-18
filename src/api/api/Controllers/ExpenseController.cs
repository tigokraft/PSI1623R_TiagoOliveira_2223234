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
            if (userId == null)
                return Unauthorized("Invalid token.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ownsCategory = await _context.Categories.AnyAsync(c => c.CategoryId == dto.CategoryId && c.UserId == userId.Value);
            if (!ownsCategory)
                return BadRequest($"Category with ID {dto.CategoryId} not found.");

            if (dto.Amount <= 0)
                return BadRequest("Amount must be positive.");

            var expense = new Expense
            {
                UserId = userId.Value,
                Amount = dto.Amount,
                Description = dto.Description,
                Date = dto.Date.Date,
                CategoryId = dto.CategoryId
            };

            await _context.Expenses.AddAsync(expense);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Expense added successfully." });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpenseDto>>> GetExpenses()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid token.");

            var expenses = await _context.Expenses
                .Where(e => e.UserId == userId.Value)
                .OrderByDescending(e => e.Date)
                .Select(e => new ExpenseDto
                {
                    ExpenseId = e.ExpenseId,
                    Amount = e.Amount,
                    Description = e.Description,
                    Date = e.Date,
                    CategoryId = e.CategoryId
                })
                .ToListAsync();

            return Ok(expenses);
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetExpenseSummary([FromQuery] string period = "monthly")
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized("Invalid user token.");

            period = period.ToLower();
            if (!new[] { "daily", "weekly", "monthly", "yearly" }.Contains(period))
                return BadRequest("Invalid period. Use 'daily', 'weekly', 'monthly', or 'yearly'.");

            DateTime fromDate = period switch
            {
                "daily" => DateTime.Today,
                "weekly" => DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek),
                "monthly" => new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                "yearly" => new DateTime(DateTime.Today.Year, 1, 1),
                _ => DateTime.Today
            };

            DateTime toDate = DateTime.Today.AddDays(1).AddTicks(-1);

            var expenses = await _context.Expenses
                .Where(e => e.UserId == userId.Value && e.Date.Date >= fromDate.Date && e.Date.Date <= toDate.Date)
                .OrderByDescending(e => e.Date)
                .Select(e => new ExpenseDto
                {
                    ExpenseId = e.ExpenseId,
                    Amount = e.Amount,
                    Description = e.Description,
                    Date = e.Date,
                    CategoryId = e.CategoryId
                })
                .ToListAsync();
        
            var totalExpense = expenses.Sum(e => e.Amount);

            return Ok(new
            {
                Period = period,
                FromDate = fromDate,
                ToDate = toDate,
                TotalExpense = totalExpense,
                Expenses = expenses
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExpenseDto>> GetExpense(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid token.");

            var expense = await _context.Expenses
                .FirstOrDefaultAsync(e => e.ExpenseId == id && e.UserId == userId.Value);

            if (expense == null) return NotFound("Expense not found.");

            var dto = new ExpenseDto
            {
                ExpenseId = expense.ExpenseId,
                Amount = expense.Amount,
                Description = expense.Description,
                Date = expense.Date,
                CategoryId = expense.CategoryId
            };

            return Ok(dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] CreateExpenseDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid token.");

            var expense = await _context.Expenses
                .FirstOrDefaultAsync(e => e.ExpenseId == id && e.UserId == userId.Value);

            if (expense == null)
                return NotFound("Expense not found.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ownsCategory = await _context.Categories.AnyAsync(c => c.CategoryId == dto.CategoryId && c.UserId == userId.Value);
            if (!ownsCategory)
                return BadRequest($"Category with ID {dto.CategoryId} not found.");

            expense.Amount = dto.Amount;
            expense.Description = dto.Description;
            expense.Date = dto.Date.Date;
            expense.CategoryId = dto.CategoryId;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Expense updated successfully." });
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
