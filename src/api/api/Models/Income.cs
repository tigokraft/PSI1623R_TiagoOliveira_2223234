using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinSync.Models
{
    public class Income
    {
        [Key]
        public int IncomeId { get; set; }

        public int UserId { get; set; }
        // public User User { get; set; } // Navigation property if you have a User model

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(255)]
        public string Descr { get; set; }
        
        public int? RecurringScheduleId { get; set; } 
        [ForeignKey("RecurringScheduleId")]
        public RecurringIncomeSchedule? RecurringSchedule { get; set; } // Navigation property

        // ⭐ NO Category navigation property here ⭐
        public int CategoryId { get; set; } // Foreign key to Category
    }
}
