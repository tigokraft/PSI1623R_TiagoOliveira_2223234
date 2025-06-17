namespace FinSync.DTOs;

public class GoalDto
{
    public int GoalId { get; set; }
    public string Name { get; set; }
    public decimal TargetAmount { get; set; }
    public decimal CurrentSaved { get; set; }
    public DateTime Deadline { get; set; }
}