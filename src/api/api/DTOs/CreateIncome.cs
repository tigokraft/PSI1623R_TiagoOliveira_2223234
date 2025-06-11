public class CreateIncomeDto
{
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
}