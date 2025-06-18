using System;

namespace FinSync.DTOs
{
    public class ExpenseDto
    {
        public int ExpenseId { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public int CategoryId { get; set; }
    }
}