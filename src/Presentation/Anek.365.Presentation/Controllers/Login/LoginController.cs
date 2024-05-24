using Anek._365.Application.Contracts;
using Anek._365.Application.Contracts.Commands.Users;
using Microsoft.AspNetCore.Mvc;

namespace Anek._365.Presentation.Controllers.Login;

[Route("")]
[PageLoad]
public class LoginController : Controller
{
    private readonly IFacadeController _facade;

    public LoginController(IFacadeController facade)
    {
        _facade = facade;
    }

    private CancellationToken CancellationToken => HttpContext.RequestAborted;

    [HttpGet("login")]
    public IActionResult Login()
    {
        return View("Login");
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        HttpContext.Response.Cookies.Delete("AAAA");
        HttpContext.Response.Cookies.Delete("Creatable");
        return RedirectToAction("GetAnekdots", "Home");
    }

    [HttpGet("signup")]
    public IActionResult SignUp()
    {
        return View("Signup", new SignUpErrorModel());
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        AuthorizeUserCommand.Response response;
        if (model.Name.Any(x => x == '@'))
        {
            response = await _facade.UserService
                .AuthorizeUserAsync(
                    new AuthorizeUserCommand.Request.ByEmail(model.Name, model.Password),
                    CancellationToken);
        }
        else
        {
            response = await _facade.UserService
                .AuthorizeUserAsync(
                    new AuthorizeUserCommand.Request.ByNickName(model.Name, model.Password),
                    CancellationToken);
        }

        if (response is AuthorizeUserCommand.Response.Success success)
        {
            HttpContext.Response.Cookies.Append("AAAA", success.Token);
            HttpContext.Response.Cookies.Append("Creatable", "+");
            return RedirectToAction("GetAnekdots", "Home");
        }
        else
        {
            return View("Login");
        }
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp(SignUpModel model)
    {
        CreateUserCommand.Response response =
            await _facade.UserService.CreateUserAsync(
                new CreateUserCommand.Request(model.Email, model.NickName, model.Password),
                CancellationToken);

        switch (response)
        {
            case CreateUserCommand.Response.Success:
            {
                AuthorizeUserCommand.Response auth = await _facade.UserService.AuthorizeUserAsync(
                    new AuthorizeUserCommand.Request.ByEmail(model.Email, model.Password),
                    CancellationToken);
                if (auth is AuthorizeUserCommand.Response.Success success)
                {
                    HttpContext.Response.Cookies.Append("AAAA", success.Token);
                    HttpContext.Response.Cookies.Append("Creatable", "+");
                    return RedirectToAction("GetAnekdots", "Home");
                }

                break;
            }

            case CreateUserCommand.Response.Failure failure:
                return View("Signup", new SignUpErrorModel(model.NickName, model.Email, failure.Message));
        }

        return View("Signup");
    }
}