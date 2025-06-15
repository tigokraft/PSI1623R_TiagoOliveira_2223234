using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinSync.Models
{
    public class Budget
    {
        [Key]
        public int BudgetId { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public decimal MonthlyLimit { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
    }
}