using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinSync.Data;
using FinSync.DTOs;
using FinSync.Models;

namespace FinSync.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GoalController : ControllerBase
    {
        private readonly FinSyncContext _context;

        public GoalController(FinSyncContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddGoal([FromBody] CreateGoalDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid token.");

            var goal = new Goal
            {
                UserId = userId.Value,
                Name = dto.Name,
                TargetAmount = dto.TargetAmount,
                Deadline = dto.Deadline
            };
            _context.Goals.Add(goal);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Goal created." });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GoalDto>>> GetGoals()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid token.");

            var goals = await _context.Goals
                .Where(g => g.UserId == userId.Value)
                .Select(g => new GoalDto
                {
                    GoalId = g.GoalId,
                    Name = g.Name,
                    TargetAmount = g.TargetAmount,
                    CurrentSaved = g.CurrentSaved,
                    Deadline = g.Deadline
                })
                .ToListAsync();

            return Ok(goals);
        }

        [HttpPost("{id}/save")]
        public async Task<IActionResult> UpdateProgress(int id, [FromBody] UpdateGoalProgressDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid token.");

            if (dto.Amount <= 0)
                return BadRequest("Amount must be positive.");

            var goal = await _context.Goals.FirstOrDefaultAsync(g => g.GoalId == id && g.UserId == userId.Value);
            if (goal == null) return NotFound();

            goal.CurrentSaved += dto.Amount;
            if (goal.CurrentSaved > goal.TargetAmount)
                goal.CurrentSaved = goal.TargetAmount;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Goal progress updated.", currentSaved = goal.CurrentSaved });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGoal(int id, [FromBody] CreateGoalDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid token.");

            var goal = await _context.Goals.FirstOrDefaultAsync(g => g.GoalId == id && g.UserId == userId.Value);
            if (goal == null) return NotFound();

            goal.Name = dto.Name;
            goal.TargetAmount = dto.TargetAmount;
            goal.Deadline = dto.Deadline;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Goal updated." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGoal(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid token.");

            var goal = await _context.Goals.FirstOrDefaultAsync(g => g.GoalId == id && g.UserId == userId.Value);
            if (goal == null) return NotFound();

            _context.Goals.Remove(goal);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Goal removed." });
        }

        private int? GetUserId()
        {
            var claim = User.FindFirst("userId")?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }
    }
}