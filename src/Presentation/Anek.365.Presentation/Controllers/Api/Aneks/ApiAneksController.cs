using Anek._365.Application.Contracts;
using Anek._365.Application.Contracts.Commands.Aneks;
using Anek._365.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anek._365.Presentation.Controllers.Api.Creating;

[ApiController]
[Route("/api/aneks")]
public class ApiAneksController : Controller
{
    private readonly IFacadeController _facade;
    private readonly CurrentUserManager _userManager;

    public ApiAneksController(IFacadeController facade, CurrentUserManager userManager)
    {
        _facade = facade;
        _userManager = userManager;
    }

    private CancellationToken CancellationToken => HttpContext.RequestAborted;

    /// <summary>
    /// Creates a new Anek with the given details.
    /// </summary>
    /// <param name="model">The creation model containing title, content, and tag IDs.</param>
    /// <returns>Returns the created Anek's ID if successful; otherwise, returns a bad request.</returns>
    /// <response code="200">Returns the new Anek ID on success.</response>
    /// <response code="400">If the creation fails for any reason.</response>
    [HttpPost("")]
    [Authorize]
    public async Task<IActionResult> CreateAnek(CreatingModel model)
    {
        CreateAnekCommand.Response response = await _facade.AneksService.CreateAnekAsync(
            new CreateAnekCommand.Request(_userManager.CurrentUser!.Id, model.Title, model.Content, model.TagIds),
            CancellationToken);

        return response switch
        {
            CreateAnekCommand.Response.Success success => Ok(success.AnekId),
            CreateAnekCommand.Response.Failure or _ => BadRequest(),
        };
    }

    /// <summary>
    /// Retrieves an Anek by its ID.
    /// </summary>
    /// <param name="id">The ID of the Anek to retrieve.</param>
    /// <returns>Returns the Anek data if found; otherwise, returns a bad request with an error message.</returns>
    /// <response code="200">Returns the Anek information if found.</response>
    /// <response code="400">If no Anek is found with the given ID or an error occurs.</response>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAnek(int id)
    {
        GetOneAnekCommand.Response response = await _facade.AneksService.GetAnekAsync(new(id), CancellationToken);
        return response switch
        {
            GetOneAnekCommand.Response.Failure failure => BadRequest(failure.Message),

            GetOneAnekCommand.Response.Success success
                => Ok(new AnekResponse(
                    success.AnekDot,
                    success.User,
                    success.Tags)),

            _ => BadRequest(),
        };
    }
}