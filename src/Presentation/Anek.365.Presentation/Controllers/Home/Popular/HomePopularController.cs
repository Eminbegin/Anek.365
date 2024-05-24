using Anek._365.Application.Contracts;
using Anek._365.Application.Contracts.Commands.Aneks;
using Anek._365.Application.Models;
using Anek._365.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Anek._365.Presentation.Controllers.Home.Popular;

[PageLoad]
[Route("popular/")]
public class HomePopularController : Controller
{
    private const int PageSize = 10;
    private const string Title = "Popular";
    private const string Action = "GetPagedPopularAnekdots";
    private const string Controller = "HomePopular";
    private static readonly (bool, bool, bool) Buttons = (false, true, false);

    private readonly IFacadeController _facade;
    private readonly CurrentUserManager _userManager;

    public HomePopularController(IFacadeController facade, CurrentUserManager userManager)
    {
        _facade = facade;
        _userManager = userManager;
    }

    private CancellationToken CancellationToken => HttpContext.RequestAborted;

    [HttpGet("{period}/{page:int}")]
    public async Task<IActionResult> GetPagedPopularAnekdots(string period, int page)
    {
        if (page == 1)
        {
            return RedirectToAction("GetPopularAnekdots");
        }

        return await GetAnekdots(page, period);
    }

    [HttpGet("{period}")]
    public async Task<IActionResult> GetPopularAnekdots(string period)
    {
        int page = 1;
        return await GetAnekdots(page, period);
    }

    private async Task<IActionResult> GetAnekdots(int page, string period)
    {
        bool isEnum = Enum.TryParse(period, true, out Period value);
        if (isEnum == false)
        {
            return RedirectToAction("GetAnekdots", "Home");
        }

        GetAneksCommand.Response response = await _facade.AneksService.GetPopularAneksAsync(
            new GetAneksCommand.Request.Popular(page, PageSize, value),
            CancellationToken);

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
                        period,
                        page,
                        Buttons,
                        PageCounter.MaxPage(PageSize, success.Count),
                        _userManager.CurrentUser)),

            GetAneksCommand.Response.Failure or _ => RedirectToAction("GetAnekdots", "Home"),
        };
    }
}