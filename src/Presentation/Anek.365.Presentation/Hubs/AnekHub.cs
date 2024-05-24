using Anek._365.Application.Contracts;
using Anek._365.Application.Contracts.Commands.Aneks;
using Microsoft.AspNetCore.SignalR;

namespace Anek._365.Presentation.Hubs;

public class AnekHub : Hub
{
    private readonly IFacadeController _facade;

    public AnekHub(IFacadeController facade)
    {
        _facade = facade;
    }

    public async Task UpdateData(int anekId)
    {
        GetOneAnekCommand.Response response = await _facade.AneksService
            .GetAnekAsync(new GetOneAnekCommand.Request(anekId), default);

        if (response is GetOneAnekCommand.Response.Success success)
        {
            await Clients.All.SendAsync("UpdateData", success.AnekDot.Mark, success.AnekDot.Views);
        }
    }
}