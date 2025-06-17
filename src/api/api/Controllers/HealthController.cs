using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FinSync.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Liveness probe: checks only application process.
    /// </summary>
    [HttpGet("live")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Live() 
        => Ok(new { status = "Alive", timestamp = DateTimeOffset.UtcNow });

    /// <summary>
    /// Readiness probe: checks DB, cache, and other dependencies.
    /// </summary>
    [HttpGet("ready")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(HealthReport), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HealthReport), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Ready([FromServices] HealthCheckService healthCheckService)
    {
        var report = await healthCheckService.CheckHealthAsync();
        var code = report.Status == HealthStatus.Healthy 
            ? StatusCodes.Status200OK 
            : StatusCodes.Status503ServiceUnavailable;
        return StatusCode(code, report);
    }
}
