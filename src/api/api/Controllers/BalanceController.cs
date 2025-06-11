using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinSync.Data;
using System.Security.Claims;

namespace FinSync.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BalanceController : ControllerBase
    {
        private readonly FinSyncContext _context;

        public BalanceController(FinSyncContext context)
        {
            _context = context;
        }

        // GET: api/balance
        [HttpGet]
        public IActionResult GetBalance()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Invalid user token.");

            var totalIncome = _context.Incomes
                .Where(i => i.UserId == userId)
                .Sum(i => (decimal?)i.Amount) ?? 0;

            var totalExpense = _context.Expenses
                .Where(e => e.UserId == userId)
                .Sum(e => (decimal?)e.Amount) ?? 0;

            var balance = totalIncome - totalExpense;

            return Ok(balance);
        }
    }
}
