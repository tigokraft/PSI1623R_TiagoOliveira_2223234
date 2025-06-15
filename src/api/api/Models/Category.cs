using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinSync.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public ICollection<Expense> Expenses { get; set; }
        public ICollection<Budget> Budgets { get; set; }
    }
}