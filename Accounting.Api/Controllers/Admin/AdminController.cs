using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Accounting.Api.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    [HttpGet("ping")]
    [Authorize(Roles = Roles.Admin)]
    public IActionResult Ping()
    {
        return Ok("admin-ok");
    }
}