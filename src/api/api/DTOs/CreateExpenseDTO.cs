using System;
using System.ComponentModel.DataAnnotations;

namespace FinSync.DTOs
{
    public class CreateExpenseDto
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be positive.")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Category ID must be a positive integer.")]
        public int CategoryId { get; set; }
    }
}