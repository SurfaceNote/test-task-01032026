using Microsoft.AspNetCore.Mvc;
using UserService.Application.Commands;
using UserService.Contracts.DTOs;

namespace UserService.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterUserCommand command,
        [FromServices] RegisterUserCommandHandler handler, CancellationToken cancellationToken)
    {
        var result = await handler.Handle(command, cancellationToken);
        
        return Ok(new AuthResponse
        {
            AccessToken = result
        });
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginUserCommand command,
        [FromServices] LoginUserCommandHandler handler, CancellationToken cancellationToken)
    {
        var result = await handler.Handle(command, cancellationToken);

        return Ok(new AuthResponse
        {
            AccessToken = result
        });
    }
}