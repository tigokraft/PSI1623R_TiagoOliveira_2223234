using System;

namespace FinSync.DTOs
{
    public class IncomeDto
    {
        public int IncomeId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? Descr { get; set; } // Marked as nullable to address CS8618 warning

        public bool IsRecurringSource { get; set; }
        public string? RecurrenceType { get; set; }
        public string CategoryName { get; set; } = null!; // Null-forgiving operator, as it will be populated.
    }
}