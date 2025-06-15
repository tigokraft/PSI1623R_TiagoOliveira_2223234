using System;

namespace FinSync.DTOs
{
    public class IncomeDto
    {
        public int IncomeId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? Descr { get; set; } 

        public bool IsRecurringSource { get; set; }
        public string? RecurrenceType { get; set; }
        public int CategoryId { get; set; } 
    }
}
