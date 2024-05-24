using Anek._365.Application.Abstractions;
using Anek._365.Application.Models;
using Anek._365.Application.Services;
using System.Security.Claims;

namespace Anek._365.Application.Application;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext httpContext,
        CurrentUserManager currentUserManager,
        IPersistenceContext persistenceContext)
    {
        Claim[] claims = httpContext.User.Claims.ToArray();
        if (claims.Any(x => x.Type == ClaimTypes.Email))
        {
            string email = claims.Single(x => x.Type == ClaimTypes.Email).Value;
            User? user = await persistenceContext.UsersRepository.GetByEmail(email, default);
            currentUserManager.CurrentUser = user;
        }

        await _next(httpContext);
    }
}