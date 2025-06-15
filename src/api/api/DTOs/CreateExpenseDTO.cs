public class CreateExpenseDto
{
    public decimal Amount { get; set; }
    public string Tags { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public int CategoryId { get; set; }
}
