using System.Collections.Generic;

namespace FinSync.DTOs
{
    public class MonthlyExpenseSummaryDto
    {
        public decimal TotalMonthlySpent { get; set; }
        public decimal TotalAllTimeSpent { get; set; }
        public List<ExpenseItemDto> Expenses { get; set; }
    }

    public class ExpenseItemDto
    {
        public int ExpenseId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string? Tags { get; set; }
        public string? CategoryName { get; set; }
    }
}
