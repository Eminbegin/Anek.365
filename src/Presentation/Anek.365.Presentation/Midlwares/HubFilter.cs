using Anek._365.Application.Abstractions;
using Anek._365.Application.Models;
using Anek._365.Application.Services;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Anek._365.Application.Application;

public class HubFilter : IHubFilter
{
    private readonly CurrentUserManager _userManager;
    private readonly IPersistenceContext _persistenceContext;

    public HubFilter(CurrentUserManager userManager, IPersistenceContext persistenceContext)
    {
        _userManager = userManager;
        _persistenceContext = persistenceContext;
    }

    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext,
        Func<HubInvocationContext, ValueTask<object?>> next)
    {
        HttpContext? httpContext = invocationContext.Context.GetHttpContext();
        Claim[] claims = httpContext!.User.Claims.ToArray();
        if (claims.Any(x => x.Type == ClaimTypes.Email))
        {
            string email = claims.Single(x => x.Type == ClaimTypes.Email).Value;
            User? user = await _persistenceContext.UsersRepository.GetByEmail(email, default);
            _userManager.CurrentUser = user;
        }

        try
        {
            return await next(invocationContext);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception calling '{invocationContext.HubMethodName}': {ex}");
            throw;
        }
    }
}
