using Anek._365.Application.Contracts;
using Anek._365.Application.Contracts.Commands.Aneks;
using Anek._365.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Anek._365.Presentation.Controllers.Home;

[PageLoad]
[Route("")]
public class HomeController : Controller
{
    private const int PageSize = 10;
    private const string Title = "Aneks";
    private const string Action = "GetPagedAnekdots";
    private const string Controller = "Home";
    private static readonly (bool, bool, bool) Buttons = (true, false, false);

    private readonly IFacadeController _facade;
    private readonly CurrentUserManager _userManager;

    public HomeController(IFacadeController facade, CurrentUserManager userManager)
    {
        _facade = facade;
        _userManager = userManager;
    }

    private CancellationToken CancellationToken => HttpContext.RequestAborted;

    [HttpGet("{page:int}")]
    public async Task<IActionResult> GetPagedAnekdots(int page)
    {
        if (page == 1)
        {
            return RedirectToAction("GetAnekdots");
        }

        return await GetAnekdots(page);
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAnekdots()
    {
        int page = 1;
        return await GetAnekdots(page);
    }

    private async Task<IActionResult> GetAnekdots(int page)
    {
        var request = new GetAneksCommand.Request.News(page, PageSize);
        GetAneksCommand.Response response = await _facade.AneksService.GetNewAneksAsync(request, CancellationToken);

        return response switch
        {
            GetAneksCommand.Response.Success success
                => View(
                    "GetAnekdots",
                    new HomeViewModel(
                        success.AnekDots.ToArray(),
                        Title,
                        Action,
                        Controller,
                        string.Empty,
                        page,
                        Buttons,
                        PageCounter.MaxPage(PageSize, success.Count),
                        _userManager.CurrentUser)),

            GetAneksCommand.Response.Failure or _ => RedirectToAction("GetAnekdots", "Home"),
        };
    }
}