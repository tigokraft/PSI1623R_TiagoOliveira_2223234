using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinSync.Data;
using FinSync.DTOs;
using FinSync.Models;
using System.Security.Claims;

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

        // POST: api/income
        [HttpPost]
        public async Task<IActionResult> AddIncome([FromBody] CreateIncomeDto dto)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Invalid user token.");

            var income = new Income
            {
                UserId = userId,
                Amount = dto.Amount,
                Date = dto.Date
            };

            _context.Incomes.Add(income);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Income added successfully." });
        }

        // GET: api/income/monthly
        [HttpGet("monthly")]
        public ActionResult<IEnumerable<Income>> GetMonthlyIncome()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Invalid user token.");

            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            var incomes = _context.Incomes
                .Where(i => i.UserId == userId && i.Date >= startOfMonth)
                .ToList();

            return Ok(incomes);
        }
    }
}
