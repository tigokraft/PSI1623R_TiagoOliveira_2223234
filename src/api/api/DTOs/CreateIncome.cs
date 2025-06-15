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
        public DateTime Date { get; set; }

        // ⭐ Removed [Required] attribute and made nullable ⭐
        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string? Descr { get; set; } 

        public bool IsRecurring { get; set; } = false;

        public string? Recurrence { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Category ID must be a positive integer.")]
        public int CategoryId { get; set; } 
    }
}
