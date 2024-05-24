using Anek._365.Application.Contracts;
using Anek._365.Application.Contracts.Commands.Users;
using Anek._365.Application.Services;
using Microsoft.AspNetCore.SignalR;

namespace Anek._365.Presentation.Hubs;

public class RateHub : Hub
{
    private readonly IFacadeController _facade;
    private readonly CurrentUserManager _userManager;

    public RateHub(IFacadeController facade, CurrentUserManager userManager)
    {
        _facade = facade;
        _userManager = userManager;
    }

    public async Task RateAnek(int anekId, int value)
    {
        if (_userManager.CurrentUser is not null)
        {
            await _facade.UserService.RateAsync(
                new RateAnekCommand.Request(anekId, _userManager.CurrentUser.Id, value),
                default);
        }
    }
}