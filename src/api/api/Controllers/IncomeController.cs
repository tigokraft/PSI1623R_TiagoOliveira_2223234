using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinSync.Data;
using FinSync.DTOs;
using FinSync.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [HttpPost]
        public async Task<IActionResult> AddIncome([FromBody] CreateIncomeDto dto)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized("Invalid token. User ID not found.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == dto.CategoryId);
            if (!categoryExists)
                return BadRequest($"Category with ID {dto.CategoryId} not found.");

            if (dto.IsRecurring)
            {
                if (string.IsNullOrWhiteSpace(dto.Recurrence) ||
                    !new[] { "daily", "weekly", "monthly", "yearly" }.Contains(dto.Recurrence.ToLower()))
                    return BadRequest("Recurrence type must be 'daily', 'weekly', 'monthly', or 'yearly'.");

                if (dto.EndDate.HasValue && dto.EndDate.Value.Date < dto.Date.Date)
                    return BadRequest("End date cannot be before start date for recurring incomes.");

                var schedule = new RecurringIncomeSchedule
                {
                    UserId = userId.Value,
                    Amount = dto.Amount,
                    Descr = dto.Descr,
                    Recurrence = dto.Recurrence.ToLower(),
                    StartDate = dto.Date.Date,
                    EndDate = dto.EndDate?.Date,
                    NextOccurrenceDate = dto.Date.Date,
                    IsActive = true
                };

                await _context.RecurringIncomeSchedules.AddAsync(schedule);
                await _context.SaveChangesAsync();

                var initialIncome = new Income
                {
                    UserId = userId.Value,
                    Amount = dto.Amount,
                    Date = dto.Date.Date,
                    Descr = dto.Descr,
                    RecurringScheduleId = schedule.ScheduleId,
                    CategoryId = dto.CategoryId
                };

                await _context.Incomes.AddAsync(initialIncome);
            }
            else
            {
                var income = new Income
                {
                    UserId = userId.Value,
                    Amount = dto.Amount,
                    Date = dto.Date.Date,
                    Descr = dto.Descr,
                    RecurringScheduleId = null,
                    CategoryId = dto.CategoryId
                };

                await _context.Incomes.AddAsync(income);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Income added successfully." });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<IncomeDto>>> GetIncomes()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid token. User ID not found.");

            var incomes = await _context.Incomes
                .Where(i => i.UserId == userId.Value)
                .Include(i => i.RecurringSchedule)
                .OrderByDescending(i => i.Date)
                .Select(i => new IncomeDto
                {
                    IncomeId = i.IncomeId,
                    Amount = i.Amount,
                    Date = i.Date,
                    Descr = i.Descr,
                    IsRecurringSource = i.RecurringScheduleId.HasValue,
                    RecurrenceType = i.RecurringSchedule != null ? i.RecurringSchedule.Recurrence : null,
                    CategoryId = i.CategoryId
                })
                .ToListAsync();

            return Ok(incomes);
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetIncomeSummary([FromQuery] string period = "monthly")
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized("Invalid token. User ID not found.");

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

            var incomes = await _context.Incomes
                .Where(i => i.UserId == userId.Value && i.Date.Date >= fromDate.Date && i.Date.Date <= toDate.Date)
                .Include(i => i.RecurringSchedule)
                .OrderByDescending(i => i.Date)
                .Select(i => new IncomeDto
                {
                    IncomeId = i.IncomeId,
                    Amount = i.Amount,
                    Date = i.Date,
                    Descr = i.Descr,
                    IsRecurringSource = i.RecurringScheduleId.HasValue,
                    RecurrenceType = i.RecurringSchedule != null ? i.RecurringSchedule.Recurrence : null,
                    CategoryId = i.CategoryId
                })
                .ToListAsync();

            var totalIncome = incomes.Sum(i => i.Amount);

            return Ok(new
            {
                Period = period,
                FromDate = fromDate,
                ToDate = toDate,
                TotalIncome = totalIncome,
                Incomes = incomes
            });
        }
        
        [HttpGet("non-recurring")]
        public async Task<ActionResult<IEnumerable<IncomeDto>>> GetNonRecurringIncomes()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid token. User ID not found.");

            var incomes = await _context.Incomes
                .Where(i => i.UserId == userId.Value && i.RecurringScheduleId == null)
                .OrderByDescending(i => i.Date)
                .Select(i => new IncomeDto
                {
                    IncomeId = i.IncomeId,
                    Amount = i.Amount,
                    Date = i.Date,
                    Descr = i.Descr,
                    IsRecurringSource = false,
                    RecurrenceType = null,
                    CategoryId = i.CategoryId
                })
                .ToListAsync();

            return Ok(incomes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<IncomeDto>>> GetIndividual(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid token. user ID not found");

            var income = await _context.Incomes
                .FirstOrDefaultAsync(i => i.IncomeId == id && i.UserId == userId.Value);

            if (income == null) return NotFound("Income not found");

            return Ok(income);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIncome(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid token. User ID not found.");

            var income = await _context.Incomes
                .FirstOrDefaultAsync(i => i.IncomeId == id && i.UserId == userId.Value);

            if (income == null) return NotFound("Income not found.");

            _context.Incomes.Remove(income);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Income deleted successfully." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIncome(int id, [FromBody] CreateIncomeDto dto)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized("Invalid token. User ID not found.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == dto.CategoryId);
            if (!categoryExists)
                return BadRequest($"Category with ID {dto.CategoryId} not found.");

            var income = await _context.Incomes
                .Include(i => i.RecurringSchedule)
                .FirstOrDefaultAsync(i => i.IncomeId == id && i.UserId == userId.Value);

            if (income == null)
                return NotFound("Income not found.");

            if (!dto.IsRecurring && income.RecurringSchedule != null)
            {
                _context.RecurringIncomeSchedules.Remove(income.RecurringSchedule);
                income.RecurringScheduleId = null;
                income.RecurringSchedule = null;
            }

            if (dto.IsRecurring)
            {
                if (string.IsNullOrWhiteSpace(dto.Recurrence) ||
                    !new[] { "daily", "weekly", "monthly", "yearly" }.Contains(dto.Recurrence.ToLower()))
                    return BadRequest("Recurrence type must be 'daily', 'weekly', 'monthly', or 'yearly'.");

                if (dto.EndDate.HasValue && dto.EndDate.Value.Date < dto.Date.Date)
                    return BadRequest("End date cannot be before start date for recurring incomes.");

                if (income.RecurringSchedule != null)
                {
                    income.RecurringSchedule.Amount = dto.Amount;
                    income.RecurringSchedule.Descr = dto.Descr;
                    income.RecurringSchedule.Recurrence = dto.Recurrence.ToLower();
                    income.RecurringSchedule.StartDate = dto.Date.Date;
                    income.RecurringSchedule.EndDate = dto.EndDate?.Date;
                    income.RecurringSchedule.NextOccurrenceDate = dto.Date.Date;
                    income.RecurringSchedule.IsActive = true;
                }
                else
                {
                    var newSchedule = new RecurringIncomeSchedule
                    {
                        UserId = userId.Value,
                        Amount = dto.Amount,
                        Descr = dto.Descr,
                        Recurrence = dto.Recurrence.ToLower(),
                        StartDate = dto.Date.Date,
                        EndDate = dto.EndDate?.Date,
                        NextOccurrenceDate = dto.Date.Date,
                        IsActive = true
                    };
                    await _context.RecurringIncomeSchedules.AddAsync(newSchedule);
                    await _context.SaveChangesAsync();

                    income.RecurringScheduleId = newSchedule.ScheduleId;
                    income.RecurringSchedule = newSchedule;
                }
            }

            income.Amount = dto.Amount;
            income.Date = dto.Date.Date;
            income.Descr = dto.Descr;
            income.CategoryId = dto.CategoryId;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Income updated successfully." });
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            return int.TryParse(userIdClaim, out var id) ? id : null;
        }
    }
}
