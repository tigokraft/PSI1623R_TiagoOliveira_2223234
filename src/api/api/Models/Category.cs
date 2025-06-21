using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinSync.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        public int UserId { get; set; }
        public string CategoryName { get; set; }

        // Stored as "R,G,B" (e.g. "255,0,0" for red)
        [Required]
        [MaxLength(11)] // "255,255,255" length
        public string Color { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public ICollection<Expense> Expenses { get; set; }
        public ICollection<Budget> Budgets { get; set; }
    }
}