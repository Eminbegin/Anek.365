using Anek._365.Application.Contracts;
using Anek._365.Application.Contracts.Commands.Users;
using Microsoft.AspNetCore.Mvc;

namespace Anek._365.Presentation.Controllers.Api.Login;

[ApiController]
[Route("/api")]
public class ApiLoginController : Controller
{
    private readonly IFacadeController _facade;

    public ApiLoginController(IFacadeController facade)
    {
        _facade = facade;
    }

    private CancellationToken CancellationToken => HttpContext.RequestAborted;

    /// <summary>
    /// Authenticates a user either by email or nickname based on the input provided.
    /// </summary>
    /// <param name="model">The login model containing the user's name and password.</param>
    /// <returns>Returns a JWT token if authentication is successful; otherwise, it returns an unauthorized or bad request status.</returns>
    /// <response code="200">Returns the JWT token on successful authentication.</response>
    /// <response code="401">If the authentication fails due to invalid credentials.</response>
    /// <response code="400">If an error occurs during the process.</response>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        AuthorizeUserCommand.Response response;
        if (model.Name.Any(x => x == '@'))
        {
            response = await _facade.UserService.AuthorizeUserAsync(
                new AuthorizeUserCommand.Request.ByEmail(model.Name, model.Password),
                CancellationToken);
        }
        else
        {
            response = await _facade.UserService.AuthorizeUserAsync(
                new AuthorizeUserCommand.Request.ByNickName(model.Name, model.Password),
                CancellationToken);
        }

        switch (response)
        {
            case AuthorizeUserCommand.Response.Success success:
                HttpContext.Response.Cookies.Append("AAAA", success.Token);
                return Ok(success.Token);
            case AuthorizeUserCommand.Response.Failure failure:
                return Unauthorized(failure.Message);
            default:
                return BadRequest();
        }
    }

    /// <summary>
    /// Registers a new user with email, nickname, and password.
    /// </summary>
    /// <param name="model">The signup model containing user details.</param>
    /// <returns>Returns the user ID if registration is successful; otherwise, it returns a bad request.</returns>
    /// <response code="200">Returns the user ID on successful registration.</response>
    /// <response code="400">If the registration fails for any reason.</response>
    [HttpPost("signup")]
    public async Task<IActionResult> SignUp(SignUpModel model)
    {
        CreateUserCommand.Response response = await _facade.UserService.CreateUserAsync(
            new CreateUserCommand.Request(model.Email, model.NickName, model.Password),
            CancellationToken);

        switch (response)
        {
            case CreateUserCommand.Response.Success success:
                await Login(new LoginModel(model.Email, model.Password));
                return Ok(success.Id);
            case CreateUserCommand.Response.Failure failure:
                return BadRequest(failure.Message);
            default:
                return BadRequest();
        }
    }

    /// <summary>
    /// Logs out the current user by deleting the authentication cookie.
    /// </summary>
    /// <returns>Returns an OK status on successful logout.</returns>
    /// <response code="200">Indicates that the logout was successful.</response>
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Response.Cookies.Delete("AAAA");
        return Ok();
    }
}