using Anek._365.Application.Contracts;
using Anek._365.Application.Contracts.Commands.Aneks;
using Microsoft.AspNetCore.SignalR;

namespace Anek._365.Presentation.Hubs;

public class AnekDataHub : Hub
{
    private readonly IFacadeController _facade;

    public AnekDataHub(IFacadeController facade)
    {
        _facade = facade;
    }

    public async Task UpdateAllData(int[] anekIds)
    {
        GetAnekListCommand.Response response = await _facade.AneksService
            .GetAneksAsync(new GetAnekListCommand.Request(anekIds), default);

        var marks = response.Aneks
            .ToDictionary(
                x => x.Id,
                x => x.Mark);

        var views = response.Aneks
            .ToDictionary(
                x => x.Id,
                x => x.Views);

        await Clients.All.SendAsync("UpdateAllData", marks, views);
    }
}