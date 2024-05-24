using Anek._365.Application.Contracts.Commands.Aneks;

namespace Anek._365.Application.Contracts.Services.Aneks;

public interface IAneksService
{
    Task<CreateAnekCommand.Response> CreateAnekAsync(
        CreateAnekCommand.Request request,
        CancellationToken cancellationToken);

    Task<GetOneAnekCommand.Response> GetAnekAsync(
        GetOneAnekCommand.Request request,
        CancellationToken cancellationToken);

    Task<GetAnekListCommand.Response> GetAneksAsync(
        GetAnekListCommand.Request request,
        CancellationToken cancellationToken);

    Task<AddViewCommand.Response> AddViewAsync(
        AddViewCommand.Request request,
        CancellationToken cancellationToken);

    Task<GetAneksCommand.Response> GetNewAneksAsync(
        GetAneksCommand.Request.News request,
        CancellationToken cancellationToken);

    Task<GetAneksCommand.Response> GetPopularAneksAsync(
        GetAneksCommand.Request.Popular request,
        CancellationToken cancellationToken);

    Task<GetAneksCommand.Response> GetMoreViewedAneksAsync(
        GetAneksCommand.Request.MoreViewed request,
        CancellationToken cancellationToken);

    Task<GetAneksCommand.Response> GetNewAneksByTagAsync(
        GetAneksCommand.Request.NewByTag request,
        CancellationToken cancellationToken);

    Task<GetAneksCommand.Response> GetPopularAneksByTagAsync(
        GetAneksCommand.Request.PopularByTag request,
        CancellationToken cancellationToken);

    Task<GetAneksCommand.Response> GetMoreViewedAneksByTagAsync(
        GetAneksCommand.Request.MoreViewedByTag request,
        CancellationToken cancellationToken);

    Task<GetAneksCommand.Response> GetNewAneksByUserAsync(
        GetAneksCommand.Request.NewByUser request,
        CancellationToken cancellationToken);

    Task<GetAneksCommand.Response> GetPopularAneksByUserAsync(
        GetAneksCommand.Request.PopularByUser request,
        CancellationToken cancellationToken);

    Task<GetAneksCommand.Response> GetMoreViewedAneksByUserAsync(
        GetAneksCommand.Request.MoreViewedByUser request,
        CancellationToken cancellationToken);
}