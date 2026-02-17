using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Api.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    [Authorize]
    [HttpGet("secure")]
    public IActionResult Secure()
    {
        return Ok(new { message = "You are authorized" });
    }
}
