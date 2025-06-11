public class CreateIncomeDto
{
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Descr { get; set; }
    public bool IsRecurring { get; set; } // new
    public string? Recurrence { get; set; } // "daily", "weekly", "monthly"
}