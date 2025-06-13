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

            if (dto.IsRecurring)
            {
                if (string.IsNullOrWhiteSpace(dto.Recurrence) ||
                    !new[] { "daily", "weekly", "monthly", "yearly" }
                        .Contains(dto.Recurrence.ToLower()))
                {
                    return BadRequest("Recurrence type must be 'daily', 'weekly', 'monthly', or 'yearly'.");
                }

                if (dto.EndDate.HasValue && dto.EndDate.Value.Date < dto.Date.Date)
                {
                    return BadRequest("End date cannot be before start date for recurring incomes.");
                }

                var schedule = new RecurringIncomeSchedule
                {
                    UserId = userId.Value,
                    Amount = dto.Amount,
                    Descr = dto.Descr,
                    Recurrence = dto.Recurrence,
                    StartDate = dto.Date.Date,
                    EndDate = dto.EndDate?.Date,
                    NextOccurrenceDate = dto.Date.Date,
                    IsActive = true
                };

                await _context.RecurringIncomeSchedules.AddAsync(schedule);
                await _context.SaveChangesAsync(); // Save to generate ScheduleId

                var initialIncome = new Income
                {
                    UserId = userId.Value,
                    Amount = dto.Amount,
                    Date = dto.Date.Date,
                    Descr = dto.Descr,
                    RecurringScheduleId = schedule.ScheduleId
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
                    Descr = dto.Descr
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
                .Where(i => i.UserId == userId)
                .Include(i => i.RecurringSchedule)
                .OrderByDescending(i => i.Date)
                .Select(i => new IncomeDto
                {
                    IncomeId = i.IncomeId,
                    Amount = i.Amount,
                    Date = i.Date,
                    Descr = i.Descr,
                    IsRecurringSource = i.RecurringScheduleId != null,
                    RecurrenceType = i.RecurringSchedule != null ? i.RecurringSchedule.Recurrence : null
                })
                .ToListAsync();

            return Ok(incomes);
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetIncomeSummary([FromQuery] string period = "")
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized("Invalid token. User ID not found.");

            period = period.ToLower();

            if (!new[] { "daily", "weekly", "monthly", "yearly", "null" }.Contains(period))
                return BadRequest("Invalid period. Use 'daily', 'weekly', 'monthly', or 'yearly'.");

            DateTime fromDate = period switch
            {
                "daily" => DateTime.Today,
                "weekly" => DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek),
                "monthly" => new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                "yearly" => new DateTime(DateTime.Today.Year, 1, 1),
                _ => DateTime.Today
            };

            DateTime toDate = DateTime.Today;

            var incomes = await _context.Incomes
                .Include(i => i.RecurringSchedule)
                .Where(i => i.UserId == userId.Value && i.Date >= fromDate && i.Date <= toDate)
                .Where(i => i.RecurringSchedule != null && i.RecurringSchedule.Recurrence.ToLower() == period)
                .OrderByDescending(i => i.Date)
                .Select(i => new IncomeDto
                {
                    IncomeId = i.IncomeId,
                    Amount = i.Amount,
                    Date = i.Date,
                    Descr = i.Descr,
                    IsRecurringSource = i.RecurringSchedule != null,
                    RecurrenceType = i.RecurringSchedule != null ? i.RecurringSchedule.Recurrence : null
                })
                .ToListAsync();

            return Ok(new
            {
                // Period = period,
                // FromDate = fromDate,
                // ToDate = toDate,
                // TotalIncome = incomes.Sum(i => i.Amount),
                Incomes = incomes
            });
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIncome(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid token. User ID not found.");

            var income = await _context.Incomes
                .FirstOrDefaultAsync(i => i.IncomeId == id && i.UserId == userId);

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
        
            var income = await _context.Incomes
                .Include(i => i.RecurringSchedule)
                .FirstOrDefaultAsync(i => i.IncomeId == id && i.UserId == userId.Value);
        
            if (income == null)
                return NotFound("Income not found.");
        
            // If the user wants to make it non-recurring
            if (!dto.IsRecurring && income.RecurringSchedule != null)
            {
                _context.RecurringIncomeSchedules.Remove(income.RecurringSchedule);
                income.RecurringScheduleId = null;
            }
        
            // If user is updating or adding recurrence
            if (dto.IsRecurring)
            {
                if (string.IsNullOrWhiteSpace(dto.Recurrence) ||
                    !new[] { "daily", "weekly", "monthly", "yearly" }.Contains(dto.Recurrence.ToLower()))
                {
                    return BadRequest("Recurrence type must be 'daily', 'weekly', 'monthly', or 'yearly'.");
                }
        
                if (dto.EndDate.HasValue && dto.EndDate.Value.Date < dto.Date.Date)
                {
                    return BadRequest("End date cannot be before start date for recurring incomes.");
                }
        
                if (income.RecurringSchedule != null)
                {
                    // Update existing schedule
                    income.RecurringSchedule.Amount = dto.Amount;
                    income.RecurringSchedule.Descr = dto.Descr;
                    income.RecurringSchedule.Recurrence = dto.Recurrence.ToLower();
                    income.RecurringSchedule.StartDate = dto.Date.Date;
                    income.RecurringSchedule.EndDate = dto.EndDate?.Date;
                    income.RecurringSchedule.NextOccurrenceDate = dto.Date.Date;
                }
                else
                {
                    // Create new recurring schedule
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
                    await _context.SaveChangesAsync(); // Save to generate ScheduleId
        
                    income.RecurringScheduleId = newSchedule.ScheduleId;
                }
            }
        
            // Common fields
            income.Amount = dto.Amount;
            income.Date = dto.Date.Date;
            income.Descr = dto.Descr;
        
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
