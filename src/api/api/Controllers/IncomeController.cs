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

        /// <summary>
        /// Adds a new income entry, which can be a one-time income or a recurring income schedule.
        /// </summary>
        /// <param name="dto">The CreateIncomeDto containing income details.</param>
        /// <returns>An action result indicating success or failure.</returns>
        [HttpPost]
        public async Task<IActionResult> AddIncome([FromBody] CreateIncomeDto dto)
        {
            var userId = GetUserId();
            if (userId == null) 
            {
                // Log unauthorized access attempt or return a more specific error
                return Unauthorized("Invalid token. User ID not found.");
            }

            // Basic validation for recurring incomes
            if (dto.IsRecurring)
            {
                // Validate recurrence type
                if (string.IsNullOrWhiteSpace(dto.Recurrence) ||
                    !(dto.Recurrence.Equals("daily", StringComparison.OrdinalIgnoreCase) ||
                      dto.Recurrence.Equals("weekly", StringComparison.OrdinalIgnoreCase) ||
                      dto.Recurrence.Equals("monthly", StringComparison.OrdinalIgnoreCase) ||
                      dto.Recurrence.Equals("yearly", StringComparison.OrdinalIgnoreCase))) 
                {
                    return BadRequest("Recurrence type must be 'daily', 'weekly', 'monthly', or 'yearly' for recurring incomes.");
                }

                // Validate end date for recurring incomes
                if (dto.EndDate.HasValue && dto.EndDate.Value.Date < dto.Date.Date)
                {
                    return BadRequest("End date cannot be before start date for recurring incomes.");
                }
            }

            if (dto.IsRecurring)
            {
                // Create the RecurringIncomeSchedule entry
                var recurringSchedule = new RecurringIncomeSchedule
                {
                    UserId = userId.Value,
                    Amount = dto.Amount,
                    Descr = dto.Descr,
                    Recurrence = dto.Recurrence!, // '!' indicates it won't be null due to prior validation
                    StartDate = dto.Date.Date, // Store date only
                    EndDate = dto.EndDate?.Date, // Store date only, if provided
                    NextOccurrenceDate = dto.Date.Date, // Initial next occurrence is the start date
                    IsActive = true
                };

                _context.RecurringIncomeSchedules.Add(recurringSchedule);
                // Save changes to get the ScheduleId before creating the initial income
                await _context.SaveChangesAsync(); 

                // Create the initial Income entry for the StartDate
                var initialIncome = new Income
                {
                    UserId = userId.Value,
                    Amount = dto.Amount,
                    Date = dto.Date.Date, // Store date only
                    Descr = dto.Descr,
                    RecurringScheduleId = recurringSchedule.ScheduleId // Link to the new schedule
                };
                _context.Incomes.Add(initialIncome);
            }
            else // One-time income
            {
                var income = new Income
                {
                    UserId = userId.Value,
                    Amount = dto.Amount,
                    Date = dto.Date.Date, // Store date only
                    Descr = dto.Descr,
                    RecurringScheduleId = null // Explicitly null for one-time incomes
                };
                _context.Incomes.Add(income);
            }
            
            await _context.SaveChangesAsync(); // Save all changes for this request

            return Ok(new { message = "Income added successfully." });
        }

        /// <summary>
        /// Retrieves all income entries for the authenticated user, including recurrence details.
        /// </summary>
        /// <returns>A list of IncomeDto objects.</returns>
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
                    RecurrenceType = i.RecurringSchedule != null ? i.RecurringSchedule.Recurrence : null
                })
                .ToListAsync();

            return Ok(incomes);
        }

        /// <summary>
        /// Retrieves a summary of income for the authenticated user based on a specified period,
        /// including recurrence details for each income.
        /// </summary>
        /// <param name="period">The period for summary (daily, weekly, monthly).</param>
        /// <returns>An object containing the summary and list of incomes.</returns>
        [HttpGet("summary")]
        public async Task<IActionResult> GetIncomeSummary([FromQuery] string period = "monthly")
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid token. User ID not found.");

            DateTime startDate;
            // Determine the start date based on the requested period
            switch (period.ToLower())
            {
                case "daily":
                    startDate = DateTime.Today; // Start of today
                    break;
                case "weekly":
                    // Start of the current week (Sunday as DayOfWeek.Sunday is 0), adjust if your week starts on Monday
                    startDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                    break;
                case "monthly":
                    // First day of the current month
                    startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    break;
                default:
                    return BadRequest("Invalid period. Use 'daily', 'weekly', or 'monthly'.");
            }

            // Fetch incomes within the specified period for the user
            var incomes = await _context.Incomes
                .Where(i => i.UserId == userId.Value && i.Date.Date >= startDate)
                // Eager load the RecurringSchedule to access its properties
                .Include(i => i.RecurringSchedule)
                .OrderByDescending(i => i.Date)
                .Select(i => new IncomeDto
                {
                    IncomeId = i.IncomeId,
                    Amount = i.Amount,
                    Date = i.Date,
                    Descr = i.Descr,
                    // Map new properties
                    IsRecurringSource = i.RecurringScheduleId.HasValue,
                    RecurrenceType = i.RecurringSchedule != null ? i.RecurringSchedule.Recurrence : null
                })
                .ToListAsync();

            var total = incomes.Sum(i => i.Amount);

            // Construct the summary result object
            var result = new
            {
                Period = period,
                FromDate = startDate,
                ToDate = DateTime.Today, // End date for summary is generally today
                TotalIncome = total,
                Incomes = incomes
            };

            return Ok(result);
        }

        /// <summary>
        /// Deletes a specific income entry for the authenticated user.
        /// </summary>
        /// <param name="id">The ID of the income entry to delete.</param>
        /// <returns>An action result indicating success or if the income was not found.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIncome(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid token. User ID not found.");

            // Find the income by ID and ensure it belongs to the authenticated user
            var income = await _context.Incomes
                .FirstOrDefaultAsync(i => i.IncomeId == id && i.UserId == userId.Value);

            if (income == null)
            {
                return NotFound("Income not found.");
            }

            // Note: This only deletes the individual income occurrence.
            // If you delete a recurring income occurrence, the recurring schedule itself remains active
            // and will continue to generate new incomes unless you provide a separate endpoint
            // to manage (deactivate/delete) RecurringIncomeSchedules.
            _context.Incomes.Remove(income);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Income deleted successfully." });
        }

        /// <summary>
        /// Helper method to extract the User ID from the authenticated user's claims.
        /// </summary>
        /// <returns>The user ID as an integer, or null if not found or invalid.</returns>
        private int? GetUserId()
        {
            // Assumes "userId" claim is present in the JWT token
            var claim = User.FindFirst("userId")?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }
    }
}
