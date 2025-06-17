namespace FinSync.DTOs;

public class BudgetDto
{
    public int BudgetId { get; set; }
    public decimal MonthlyLimit { get; set; }
    public int CategoryId { get; set; }
}