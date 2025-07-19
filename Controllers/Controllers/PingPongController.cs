using Microsoft.AspNetCore.Mvc;
using BusinessObjects.Common;

namespace Controllers.Controllers;

/// <summary>
/// Controller mẫu để kiểm tra SwaggerUI
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PingPongController : ControllerBase
{
    [HttpGet("ping")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Ping()
    {
        return Ok(new ApiResponse{
            Code = 200,
            Status = true,
            Message = "Skibidi bop bop"
        });
    }
} 