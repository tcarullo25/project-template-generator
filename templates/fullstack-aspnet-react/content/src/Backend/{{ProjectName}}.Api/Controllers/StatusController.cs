using Microsoft.AspNetCore.Mvc;
using {{ProjectNamespace}}.Application.Status;

namespace {{ProjectNamespace}}.Api.Controllers;

/// <summary>
/// The template's one endpoint. It exists so a newly generated project can
/// demonstrate React -> ASP.NET Core -> PostgreSQL working; delete it once real
/// endpoints exist.
/// </summary>
[ApiController]
[Route("api/status")]
public sealed class StatusController : ControllerBase
{
    private readonly StatusService _statusService;

    public StatusController(StatusService statusService)
    {
        _statusService = statusService;
    }

    [HttpGet]
    [ProducesResponseType<ApplicationStatus>(StatusCodes.Status200OK)]
    public async Task<ActionResult<ApplicationStatus>> GetAsync(CancellationToken cancellationToken)
    {
        return await _statusService.GetStatusAsync(cancellationToken);
    }
}
