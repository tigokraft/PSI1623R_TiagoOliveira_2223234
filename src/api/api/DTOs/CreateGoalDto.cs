namespace FinSync.DTOs;

public class CreateGoalDto
{
    public string Name { get; set; }
    public decimal TargetAmount { get; set; }
    public DateTime Deadline { get; set; }
}