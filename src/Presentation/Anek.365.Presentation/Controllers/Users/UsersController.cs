using Anek._365.Application.Contracts;
using Anek._365.Application.Contracts.Commands.Aneks;
using Anek._365.Application.Contracts.Commands.Users;
using Anek._365.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Anek._365.Presentation.Controllers.Users;

[PageLoad]
[Route("")]
public class UsersController : Controller
{
    private const int PageSize = 10;
    private const string Controller = "Users";

    private readonly IFacadeController _facade;
    private readonly CurrentUserManager _userManager;

    public UsersController(IFacadeController facade, CurrentUserManager userManager)
    {
        _facade = facade;
        _userManager = userManager;
    }

    private CancellationToken CancellationToken => HttpContext.RequestAborted;

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        return await GetUserList(1);
    }

    [HttpGet("users/{page:int}")]
    public async Task<IActionResult> GetPagedUsers(int page)
    {
        if (page == 1)
        {
            return RedirectToAction("GetUsers");
        }

        return await GetUserList(page);
    }

    [HttpGet("/user/{name}")]
    public async Task<IActionResult> GetNew(string name)
    {
        return await GetNewAneks(name, 1);
    }

    [HttpGet("/user/{name}/{page:int}")]
    public async Task<IActionResult> GetPagedNew(string name, int page)
    {
        if (page == 1)
        {
            return RedirectToAction("GetNew", new { name });
        }

        return await GetNewAneks(name, page);
    }

    [HttpGet("/user/{name}/popular")]
    public async Task<IActionResult> GetPopular(string name)
    {
        return await GetPopularAneks(name, 1);
    }

    [HttpGet("/user/{name}/popular/{page:int}")]
    public async Task<IActionResult> GetPagedPopular(string name, int page)
    {
        if (page == 1)
        {
            return RedirectToAction("GetPopular", new { name });
        }

        return await GetPopularAneks(name, page);
    }

    [HttpGet("/user/{name}/more-viewed")]
    public async Task<IActionResult> GetMoreViewed(string name)
    {
        return await GetMoreViewedAneks(name, 1);
    }

    [HttpGet("/user/{name}/more-viewed/{page:int}")]
    public async Task<IActionResult> GetPagedMoreViewed(string name, int page)
    {
        if (page == 1)
        {
            return RedirectToAction("GetMoreViewed", new { name });
        }

        return await GetMoreViewedAneks(name, page);
    }

    private async Task<IActionResult> GetNewAneks(string name, int page)
    {
        GetAneksCommand.Response response = await _facade.AneksService
            .GetNewAneksByUserAsync(
                new(name, page, PageSize),
                CancellationToken);

        return response switch
        {
            GetAneksCommand.Response.Success success
                => View(
                    "GetUserAneks",
                    new UserAneksModel(
                        success.AnekDots.ToArray(),
                        $"Новые - {success.Name}",
                        "GetPagedNew",
                        Controller,
                        name,
                        page,
                        (true, false, false),
                        PageCounter.MaxPage(PageSize, success.Count),
                        _userManager.CurrentUser)),

            GetAneksCommand.Response.Failure or _ => RedirectToAction("GetAnekdots", "Home"),
        };
    }

    private async Task<IActionResult> GetPopularAneks(string name, int page)
    {
        GetAneksCommand.Response response = await _facade.AneksService
            .GetPopularAneksByUserAsync(
                new(name, page, PageSize),
                CancellationToken);

        return response switch
        {
            GetAneksCommand.Response.Success success
                => View(
                    "GetUserAneks",
                    new UserAneksModel(
                        success.AnekDots.ToArray(),
                        $"Популярное - {success.Name}",
                        "GetPagedPopular",
                        Controller,
                        name,
                        page,
                        (false, true, false),
                        PageCounter.MaxPage(PageSize, success.Count),
                        _userManager.CurrentUser)),

            GetAneksCommand.Response.Failure or _ => RedirectToAction("GetAnekdots", "Home"),
        };
    }

    private async Task<IActionResult> GetMoreViewedAneks(string name, int page)
    {
        GetAneksCommand.Response response = await _facade.AneksService
            .GetMoreViewedAneksByUserAsync(
                new(name, page, PageSize),
                CancellationToken);

        return response switch
        {
            GetAneksCommand.Response.Success success
                => View(
                    "GetUserAneks",
                    new UserAneksModel(
                        success.AnekDots.ToArray(),
                        $"Больше просмотров - {success.Name}",
                        "GetPagedMoreViewed",
                        Controller,
                        name,
                        page,
                        (false, false, true),
                        PageCounter.MaxPage(PageSize, success.Count),
                        _userManager.CurrentUser)),

            GetAneksCommand.Response.Failure or _ => RedirectToAction("GetAnekdots", "Home"),
        };
    }

    private async Task<IActionResult> GetUserList(int page)
    {
        GetUsersCommand.Response response = await _facade.UserService.GetUsersAsync(new(page, int.MaxValue), CancellationToken);

        var model = new UserListModel(
            response.Users,
            page,
            "Users",
            "GetPagedUsers",
            PageCounter.MaxPage(int.MaxValue, response.Count),
            _userManager.CurrentUser);

        return View("GetUsers", model);
    }
}