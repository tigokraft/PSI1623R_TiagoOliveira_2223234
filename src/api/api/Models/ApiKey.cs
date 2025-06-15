using System;
using System.ComponentModel.DataAnnotations;

namespace FinSync.Models
{
    public class ApiKey
    {
        [Key]
        public int Id { get; set; }
        public string Key { get; set; }
        public string Owner { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}