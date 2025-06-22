using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FinSync.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthService;

    public HealthController(HealthCheckService healthService)
    {
        _healthService = healthService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get()
    {
        var report = await _healthService.CheckHealthAsync();
        var details = report.Entries.ToDictionary(e => e.Key, e => e.Value.Status.ToString());
        return Ok(new
        {
            status = report.Status.ToString(),
            details
        });
    }
}