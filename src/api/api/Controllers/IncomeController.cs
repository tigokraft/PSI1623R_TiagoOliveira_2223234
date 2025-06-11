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
    public class IncomeController : ControllerBase
    {
        private readonly FinSyncContext _context;

        public IncomeController(FinSyncContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddIncome([FromBody] CreateIncomeDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid token.");

            var income = new Income
            {
                UserId = userId.Value,
                Amount = dto.Amount,
                Date = dto.Date,
                Descr = dto.Descr
            };

            _context.Incomes.Add(income);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Income added successfully." });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<IncomeDto>>> GetIncomes()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid token.");

            var incomes = await _context.Incomes
                .Where(i => i.UserId == userId.Value)
                .OrderByDescending(i => i.Date)
                .Select(i => new IncomeDto
                {
                    IncomeId = i.IncomeId,
                    Amount = i.Amount,
                    Date = i.Date,
                    Descr = i.Descr
                })
                .ToListAsync();

            return Ok(incomes);
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetIncomeSummary([FromQuery] string period = "monthly")
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid token.");

            DateTime startDate = period.ToLower() switch
            {
                "daily" => DateTime.Today,
                "weekly" => DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek),
                "monthly" => new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                _ => throw new ArgumentException("Invalid period. Use 'daily', 'weekly', or 'monthly'.")
            };

            var incomes = await _context.Incomes
                .Where(i => i.UserId == userId.Value && i.Date >= startDate)
                .OrderByDescending(i => i.Date)
                .Select(i => new IncomeDto
                {
                    IncomeId = i.IncomeId,
                    Amount = i.Amount,
                    Date = i.Date,
                    Descr = i.Descr
                })
                .ToListAsync();

            var total = incomes.Sum(i => i.Amount);

            var result = new
            {
                Period = period,
                FromDate = startDate,
                ToDate = DateTime.Now,
                TotalIncome = total,
                Incomes = incomes
            };

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIncome(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid token.");

            var income = await _context.Incomes
                .FirstOrDefaultAsync(i => i.IncomeId == id && i.UserId == userId.Value);

            if (income == null)
                return NotFound("Income not found.");

            _context.Incomes.Remove(income);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Income deleted successfully." });
        }

        private int? GetUserId()
        {
            var claim = User.FindFirst("userId")?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }
    }
}
