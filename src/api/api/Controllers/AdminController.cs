using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinSync.Data;
using FinSync.Models;
using System.Linq;

namespace FinSync.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class AdminController : ControllerBase
{
    private readonly FinSyncContext _context;

    public AdminController(FinSyncContext context)
    {
        _context = context;
    }

    [HttpGet("users")]
    public IActionResult GetUsers()
    {
        var users = _context.Users.Select(u => new { u.UserId, u.Username, u.Role }).ToList();
        return Ok(users);
    }

    [HttpPost("promote/{id}")]
    public IActionResult Promote(int id)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserId == id);
        if (user == null)
            return NotFound();

        user.Role = "admin";
        _context.SaveChanges();
        return NoContent();
    }
}
