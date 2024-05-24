using Anek._365.Application.Contracts;
using Anek._365.Application.Contracts.Commands.Aneks;
using Anek._365.Application.Models;
using Anek._365.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Anek._365.Presentation.Controllers.Home.MoreViewed;

[PageLoad]
[Route("more-viewed/")]
public class HomeMoreViewController : Controller
{
    private const int PageSize = 10;
    private const string Title = "More Viewed";
    private const string Action = "GetPagedMoreViewedAnekdots";
    private const string Controller = "HomeMoreView";
    private static readonly (bool, bool, bool) Buttons = (false, false, true);

    private readonly IFacadeController _facade;
    private readonly CurrentUserManager _userManager;

    public HomeMoreViewController(IFacadeController facade, CurrentUserManager userManager)
    {
        _facade = facade;
        _userManager = userManager;
    }

    private CancellationToken CancellationToken => HttpContext.RequestAborted;

    [HttpGet("{period}/{page:int}")]
    public async Task<IActionResult> GetPagedMoreViewedAnekdots(string period, int page)
    {
        return page == 1
            ? RedirectToAction("GetMoreViewedAnekdots")
            : await GetAnekdots(page, period);
    }

    [HttpGet("{period}")]
    public async Task<IActionResult> GetMoreViewedAnekdots(string period)
    {
        return await GetAnekdots(1, period);
    }

    private async Task<IActionResult> GetAnekdots(int page, string period)
    {
        bool isEnum = Enum.TryParse(period, true, out Period value);
        if (isEnum == false)
        {
            return RedirectToAction("GetAnekdots", "Home");
        }

        GetAneksCommand.Response response = await _facade.AneksService.GetMoreViewedAneksAsync(
            new GetAneksCommand.Request.MoreViewed(page, PageSize, value),
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