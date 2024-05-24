using Anek._365.Application.Contracts;
using Anek._365.Application.Contracts.Commands.Aneks;
using Anek._365.Application.Contracts.Commands.Tags;
using Anek._365.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Anek._365.Presentation.Controllers.Tags;

[PageLoad]
[Route("/tags")]
public class TagsController : Controller
{
    private const int PageSize = 10;
    private const string Controller = "Tags";

    private readonly IFacadeController _facade;
    private readonly CurrentUserManager _userManager;

    public TagsController(IFacadeController facade, CurrentUserManager userManager)
    {
        _facade = facade;
        _userManager = userManager;
    }

    private CancellationToken CancellationToken => HttpContext.RequestAborted;

    [HttpGet("")]
    public async Task<IActionResult> GetTags()
    {
        GetAllTagsWithViewsCommand.Response response = await _facade.TagsService
            .GetAllTagsWithViewsAsync(
                new(),
                CancellationToken);

        var model = new TagsListModel(response.Tags.ToArray(), _userManager.CurrentUser);
        return View("GetTags", model);
    }

    [HttpGet("/{name}")]
    public async Task<IActionResult> GetNew(string name)
    {
        return await GetNewAneks(name, 1);
    }

    [HttpGet("/{name}/{page:int}")]
    public async Task<IActionResult> GetPagedNew(string name, int page)
    {
        if (page == 1)
        {
            return RedirectToAction("GetNew", new { name });
        }

        return await GetNewAneks(name, page);
    }

    [HttpGet("/{name}/popular")]
    public async Task<IActionResult> GetPopular(string name)
    {
        return await GetPopularAneks(name, 1);
    }

    [HttpGet("/{name}/popular/{page:int}")]
    public async Task<IActionResult> GetPagedPopular(string name, int page)
    {
        if (page == 1)
        {
            return RedirectToAction("GetPopular", new { name });
        }

        return await GetPopularAneks(name, page);
    }

    [HttpGet("/{name}/more-viewed")]
    public async Task<IActionResult> GetMoreViewed(string name)
    {
        return await GetMoreViewedAneks(name, 1);
    }

    [HttpGet("/{name}/more-viewed/{page:int}")]
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
            .GetNewAneksByTagAsync(
                new(name, page, PageSize),
                CancellationToken);

        return response switch
        {
            GetAneksCommand.Response.Success success
                => View(
                    "GetTagAneks",
                    new TagAneksModel(
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
            .GetPopularAneksByTagAsync(
                new(name, page, PageSize),
                CancellationToken);

        return response switch
        {
            GetAneksCommand.Response.Success success
                => View(
                    "GetTagAneks",
                    new TagAneksModel(
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
            .GetMoreViewedAneksByTagAsync(
                new(name, page, PageSize),
                CancellationToken);

        return response switch
        {
            GetAneksCommand.Response.Success success
                => View(
                    "GetTagAneks",
                    new TagAneksModel(
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
}