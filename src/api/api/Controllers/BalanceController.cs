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
            Console.WriteLine($"[DEBUG] userIdClaim = {userIdClaim}");
        
            if (!int.TryParse(userIdClaim, out int userId))
            {
                Console.WriteLine("[DEBUG] Invalid userIdClaim, returning Unauthorized");
                return Unauthorized("Invalid user token.");
            }
        
            Console.WriteLine($"[DEBUG] Parsed userId = {userId}");
        
            var totalIncome = _context.Incomes
                .Where(i => i.UserId == userId)
                .Sum(i => (decimal?)i.Amount) ?? 0;
            Console.WriteLine($"[DEBUG] Total income = {totalIncome}");
        
            var totalExpense = _context.Expenses
                .Where(e => e.UserId == userId)
                .Sum(e => (decimal?)e.Amount) ?? 0;
            Console.WriteLine($"[DEBUG] Total expense = {totalExpense}");
        
            var balance = totalIncome - totalExpense;
            Console.WriteLine($"[DEBUG] Balance = {balance}");
        
            return Ok(balance);
        }

    }
}
