using Anek._365.Application.Contracts;
using Anek._365.Application.Contracts.Commands.Aneks;
using Anek._365.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Anek._365.Presentation.Controllers.Api;

[ApiController]
[Route("/api")]
public class ApiHomeController : Controller
{
    private const int PageSize = 10;
    private readonly IFacadeController _facade;

    public ApiHomeController(IFacadeController facade)
    {
        _facade = facade;
    }

    private CancellationToken CancellationToken => HttpContext.RequestAborted;

    /// <summary>
    /// Retrieves a paginated list of new aneks.
    /// </summary>
    /// <param name="page">The page number to retrieve.</param>
    /// <returns>A list of aneks for the specified page.</returns>
    [HttpGet("{page:int}")]
    public async Task<IActionResult> GetPaged(int page)
    {
        return await GetAneks(page);
    }

    /// <summary>
    /// Retrieves the first page of new aneks.
    /// </summary>
    /// <returns>A list of the newest aneks on the first page.</returns>
    [HttpGet("")]
    public async Task<IActionResult> Get()
    {
        return await GetAneks(1);
    }

    /// <summary>
    /// Retrieves a paginated list of popular aneks for a specified period.
    /// </summary>
    /// <param name="period">The time period (e.g., 'Daily', 'Weekly', 'Monthly').</param>
    /// <param name="page">The page number to retrieve.</param>
    /// <returns>A list of popular aneks for the specified period and page.</returns>
    [HttpGet("popular/{period}/{page:int}")]
    public async Task<IActionResult> GetPopularPaged(string period, int page)
    {
        return await GetPopularAneks(period, page);
    }

    /// <summary>
    /// Retrieves the first page of popular aneks for a specified period.
    /// </summary>
    /// <param name="period">The time period (e.g., 'Daily', 'Weekly', 'Monthly').</param>
    /// <returns>A list of popular aneks for the first page of the specified period.</returns>
    [HttpGet("popular/{period}")]
    public async Task<IActionResult> GetPopular(string period)
    {
        return await GetPopularAneks(period, 1);
    }

    /// <summary>
    /// Retrieves a paginated list of the most viewed aneks for a specified period.
    /// </summary>
    /// <param name="period">The time period (e.g., 'Daily', 'Weekly', 'Monthly').</param>
    /// <param name="page">The page number to retrieve.</param>
    /// <returns>A list of the most viewed aneks for the specified period and page.</returns>
    [HttpGet("more-viewed/{period}/{page:int}")]
    public async Task<IActionResult> GetMoreViewedPaged(string period, int page)
    {
        return await GetMoreViewedAneks(period, page);
    }

    /// <summary>
    /// Retrieves the first page of the most viewed aneks for a specified period.
    /// </summary>
    /// <param name="period">The time period (e.g., 'Daily', 'Weekly', 'Monthly').</param>
    /// <returns>A list of the most viewed aneks for the first page of the specified period.</returns>
    [HttpGet("more-viewed/{period}")]
    public async Task<IActionResult> GetMoreViewed(string period)
    {
        return await GetMoreViewedAneks(period, 1);
    }

    private async Task<IActionResult> GetAneks(int page)
    {
        GetAneksCommand.Response response = await _facade.AneksService.GetNewAneksAsync(
            new GetAneksCommand.Request.News(page, PageSize),
            CancellationToken);

        return response switch
        {
            GetAneksCommand.Response.Success success => Ok(success.AnekDots),
            GetAneksCommand.Response.Failure or _ => BadRequest(),
        };
    }

    private async Task<IActionResult> GetPopularAneks(string period, int page)
    {
        bool isEnum = Enum.TryParse(period, true, out Period value);
        if (isEnum == false)
        {
            return BadRequest();
        }

        GetAneksCommand.Response response = await _facade.AneksService.GetPopularAneksAsync(
            new GetAneksCommand.Request.Popular(page, PageSize, value),
            CancellationToken);

        return response switch
        {
            GetAneksCommand.Response.Success success => Ok(success.AnekDots),
            GetAneksCommand.Response.Failure or _ => BadRequest(),
        };
    }

    private async Task<IActionResult> GetMoreViewedAneks(string period, int page)
    {
        bool isEnum = Enum.TryParse(period, true, out Period value);
        if (isEnum == false)
        {
            return BadRequest();
        }

        GetAneksCommand.Response response = await _facade.AneksService.GetMoreViewedAneksAsync(
            new GetAneksCommand.Request.MoreViewed(page, PageSize, value),
            CancellationToken);

        return response switch
        {
            GetAneksCommand.Response.Success success => Ok(success.AnekDots),
            GetAneksCommand.Response.Failure or _ => BadRequest(),
        };
    }
}