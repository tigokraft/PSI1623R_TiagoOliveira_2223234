using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinSync.Models
{
    public class Goal
    {
        [Key]
        public int GoalId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentSaved { get; set; } = 0;
        public DateTime Deadline { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}