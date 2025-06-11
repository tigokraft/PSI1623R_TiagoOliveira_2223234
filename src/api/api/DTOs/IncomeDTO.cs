using System;

namespace FinSync.DTOs
{
    public class IncomeDto
    {
        public int IncomeId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Descr { get; set; }

        // New properties to indicate if it's from a recurring source and its type
        public bool IsRecurringSource { get; set; }
        public string? RecurrenceType { get; set; } // Nullable, as one-time incomes won't have this
    }
}
