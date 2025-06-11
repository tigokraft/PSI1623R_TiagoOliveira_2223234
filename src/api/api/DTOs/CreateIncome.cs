using System;
using System.ComponentModel.DataAnnotations;

namespace FinSync.DTOs
{
    public class CreateIncomeDto
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be positive.")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; } // This will be the StartDate for recurring incomes, or the Date for one-time incomes

        [Required]
        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string Descr { get; set; }

        public bool IsRecurring { get; set; } = false; // Default to false

        // Only required if IsRecurring is true
        public string? Recurrence { get; set; } // "daily", "weekly", "monthly", "yearly"

        public DateTime? EndDate { get; set; } // Only for recurring incomes, can be null
    }
}