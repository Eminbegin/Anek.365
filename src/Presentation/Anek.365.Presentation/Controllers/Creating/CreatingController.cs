using Anek._365.Application.Contracts;
using Anek._365.Application.Contracts.Commands.Aneks;
using Anek._365.Application.Contracts.Commands.Tags;
using Anek._365.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anek._365.Presentation.Controllers.Creating;

[PageLoad]
[Route("create/")]
public class CreatingController : Controller
{
    private readonly IFacadeController _facade;
    private readonly CurrentUserManager _userManager;

    public CreatingController(IFacadeController facade, CurrentUserManager userManager)
    {
        _facade = facade;
        _userManager = userManager;
    }

    private CancellationToken CancellationToken => HttpContext.RequestAborted;

    [Authorize]
    [HttpGet("aneks")]
    public async Task<IActionResult> CreateAnekdot()
    {
        GetAllTagsCommand.Response response = await _facade.TagsService.GetAllTagsAsync(new(), CancellationToken);

        return View(
            "CreatingAnekdot",
            new CreatingAnekdotViewModel(
                null,
                null,
                response.Tags.ToArray(),
                _userManager.CurrentUser));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAnek(string? title, string? content, string? tags)
    {
        if (title is null || content is null)
        {
            GetAllTagsCommand.Response tagsResponse = await _facade.TagsService
                .GetAllTagsAsync(new(), CancellationToken);

            return RedirectToAction(
                "CreateAnekdot",
                "Creating",
                new CreatingAnekdotViewModel(title, content, tagsResponse.Tags.ToArray(), _userManager.CurrentUser));
        }

        string? creatable = HttpContext.Request.Cookies["Creatable"];
        HttpContext.Response.Cookies.Append("Creatable", "-");
        if (creatable == "+")
        {
            var request = new CreateAnekCommand.Request(
                _userManager.CurrentUser?.Id ?? 1,
                title,
                content,
                tags is not null ? tags.Split(' ').Select(int.Parse) : []);

            CreateAnekCommand.Response
                response = await _facade.AneksService.CreateAnekAsync(request, CancellationToken);

            HttpContext.Response.Cookies.Append("Creatable", "+");

            return response switch
            {
                CreateAnekCommand.Response.Failure
                    => RedirectToAction("CreateAnekdot", "Creating"),

                CreateAnekCommand.Response.Success success
                    => RedirectToAction("GetAnekdot", "Anekdots", new { id = success.AnekId }),

                _ => throw new ArgumentException("asd"),
            };
        }

        return RedirectToAction("GetAnekdots", "Home");
    }
}