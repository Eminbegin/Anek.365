using Anek._365.Application.Contracts;
using Anek._365.Application.Contracts.Commands.Aneks;
using Anek._365.Application.Contracts.Commands.Users;
using Anek._365.Application.Models;
using Anek._365.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anek._365.Presentation.Controllers.Anekdots;

[PageLoad]
[Route("/aneks")]
public class AnekdotsController : Controller
{
    private readonly IFacadeController _facade;
    private readonly CurrentUserManager _userManager;

    public AnekdotsController(IFacadeController facade, CurrentUserManager userManager)
    {
        _facade = facade;
        _userManager = userManager;
    }

    private CancellationToken CancellationToken => HttpContext.RequestAborted;

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAnekdot(int id)
    {
        GetOneAnekCommand.Response response = await _facade.AneksService.GetAnekAsync(new(id), CancellationToken);
        switch (response)
        {
            case GetOneAnekCommand.Response.Failure failure:
                throw new ArgumentException(failure.Message);

            case GetOneAnekCommand.Response.Success success:
                GetAnekdotViewModel model = ConvertToModel(success.AnekDot, success.User, success.Tags);
                await AddView(id);
                return View("Anekdot", model);

            default:
                throw new ArgumentException("Hoho");
        }
    }

    [Authorize]
    [HttpGet("rate/{anekId:int}/{mark:int}")]
    public async Task<IActionResult> RateAnek(int anekId, int mark)
    {
        if (_userManager.CurrentUser is not null)
        {
            await _facade.UserService.RateAsync(
                new RateAnekCommand.Request(anekId, _userManager.CurrentUser.Id, mark),
                CancellationToken);
        }

        return RedirectToAction("GetAnekdot", "Anekdots", new { id = anekId });
    }

    private async Task AddView(int anekId)
    {
        if (_userManager.CurrentUser is null)
        {
            string? viewedAneks = HttpContext.Request.Cookies["ViewedAneks"];
            if (viewedAneks is null)
            {
                HttpContext.Response.Cookies.Append("ViewedAneks", $"{anekId}");
                await _facade.AneksService.AddViewAsync(new AddViewCommand.Request(anekId), CancellationToken);
            }
            else
            {
                if (viewedAneks.Split(' ').Select(int.Parse).Contains(anekId) is false)
                {
                    HttpContext.Response.Cookies.Append("ViewedAneks", $"{viewedAneks} {anekId}");
                    await _facade.AneksService.AddViewAsync(new AddViewCommand.Request(anekId), CancellationToken);
                }
            }
        }
        else
        {
            await _facade.UserService.AddViewAsync(
                new AddViewUserCommand.Request(anekId, _userManager.CurrentUser.Id),
                CancellationToken);
        }
    }

    private GetAnekdotViewModel ConvertToModel(AnekDot anek, User user, Tag[] tags)
    {
        return new GetAnekdotViewModel(
            anek.Title,
            anek.Text,
            anek.Id,
            DateOnly.FromDateTime(anek.CreatedAt.Date),
            anek.Mark,
            anek.Views,
            tags,
            user,
            _userManager.CurrentUser);
    }
}