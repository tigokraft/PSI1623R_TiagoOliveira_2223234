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
            _log.LogInformation("Recurring income check started at {Time}", DateTime.Now);

            var recurring = await _context.Incomes.Where(i => i.IsRecurring).ToListAsync();

            foreach (var inc in recurring)
            {
                bool shouldGenerate = inc.Recurrence switch
                {
                    "daily" => inc.Date.Date < DateTime.Today,
                    "weekly" => inc.Date.Date.AddDays(7) <= DateTime.Today,
                    "monthly" => inc.Date.Date.AddMonths(1) <= DateTime.Today,
                    _ => false
                };

                if (shouldGenerate)
                {
                    var nextDate = inc.Recurrence switch
                    {
                        "daily" => inc.Date.AddDays(1),
                        "weekly" => inc.Date.AddDays(7),
                        "monthly" => inc.Date.AddMonths(1),
                        _ => inc.Date
                    };

                    var next = new Income
                    {
                        UserId = inc.UserId,
                        Amount = inc.Amount,
                        Date = nextDate,
                        Descr = inc.Descr + " (recurring)",
                        IsRecurring = true,
                        Recurrence = inc.Recurrence
                    };

                    _context.Incomes.Add(next);
                    inc.Date = nextDate;

                    _log.LogInformation("Recurring income generated for user {UserId} at {Date}", inc.UserId, nextDate);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
