// File: FinSync/Jobs/RecurringIncomeJob.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Microsoft.EntityFrameworkCore;
using FinSync.Data;
using FinSync.Models;

namespace FinSync.Jobs
{
    public class RecurringIncomeJob : IJob
    {
        private readonly FinSyncContext _context;
        private readonly ILogger<RecurringIncomeJob> _log;

        public RecurringIncomeJob(FinSyncContext context, ILogger<RecurringIncomeJob> log)
        {
            _context = context;
            _log = log;
        }

        public async Task Execute(IJobExecutionContext ctx)
        {
            _log.LogInformation("Recurring income generation job started at {Time}", DateTime.Now);

            var today = DateTime.UtcNow.Date; // Using UTC for consistency

            var dueSchedules = await _context.RecurringIncomeSchedules // This will now be recognized
                .Where(s => s.IsActive && s.NextOccurrenceDate.Date <= today)
                .ToListAsync();

            _log.LogInformation("Found {Count} recurring income schedules due for generation.", dueSchedules.Count);

            foreach (var schedule in dueSchedules)
            {
                try
                {
                    var existingIncome = await _context.Incomes.FirstOrDefaultAsync(
                        i => i.RecurringScheduleId == schedule.ScheduleId && i.Date.Date == schedule.NextOccurrenceDate.Date
                    );

                    if (existingIncome != null)
                    {
                        _log.LogWarning("Income for schedule {ScheduleId} on {Date} already exists. Skipping generation.", schedule.ScheduleId, schedule.NextOccurrenceDate.Date);
                        UpdateScheduleNextOccurrence(schedule); // Still advance the schedule's next date
                        continue;
                    }

                    var newIncome = new Income
                    {
                        UserId = schedule.UserId,
                        Amount = schedule.Amount,
                        Date = schedule.NextOccurrenceDate.Date,
                        Descr = schedule.Descr + " (recurring)",
                        RecurringScheduleId = schedule.ScheduleId
                    };

                    _context.Incomes.Add(newIncome);
                    _log.LogInformation("Generated new income for user {UserId} from schedule {ScheduleId} for {Date} (Amount: {Amount})", 
                                        newIncome.UserId, schedule.ScheduleId, newIncome.Date, newIncome.Amount);

                    UpdateScheduleNextOccurrence(schedule);

                    if (schedule.EndDate.HasValue && schedule.NextOccurrenceDate.Date > schedule.EndDate.Value.Date)
                    {
                        schedule.IsActive = false;
                        _log.LogInformation("Recurring schedule {ScheduleId} deactivated as it passed its end date {EndDate}.", schedule.ScheduleId, schedule.EndDate.Value.Date);
                    }
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "Error processing recurring income schedule {ScheduleId}: {Message}", schedule.ScheduleId, ex.Message);
                }
            }

            await _context.SaveChangesAsync();
            _log.LogInformation("Recurring income generation job finished at {Time}", DateTime.Now);
        }

        private void UpdateScheduleNextOccurrence(RecurringIncomeSchedule schedule)
        {
            switch (schedule.Recurrence.ToLower())
            {
                case "daily":
                    schedule.NextOccurrenceDate = schedule.NextOccurrenceDate.AddDays(1);
                    break;
                case "weekly":
                    schedule.NextOccurrenceDate = schedule.NextOccurrenceDate.AddDays(7);
                    break;
                case "monthly":
                    schedule.NextOccurrenceDate = schedule.NextOccurrenceDate.AddMonths(1);
                    break;
                case "yearly":
                    schedule.NextOccurrenceDate = schedule.NextOccurrenceDate.AddYears(1);
                    break;
                default:
                    _log.LogWarning("Unknown recurrence type '{Recurrence}' for schedule {ScheduleId}. Schedule will not advance.", schedule.Recurrence, schedule.ScheduleId);
                    schedule.IsActive = false;
                    break;
            }
        }
    }
}