using System.ComponentModel.DataAnnotations;

namespace FinSync.Models
{
    public class Income
    {
        public int IncomeId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Descr { get; set; }
        public User User { get; set; }
    }
}