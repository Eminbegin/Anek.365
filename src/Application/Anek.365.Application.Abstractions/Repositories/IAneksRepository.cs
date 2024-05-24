using Anek._365.Application.Abstractions.Queries.Aneks;
using Anek._365.Application.Models;

namespace Anek._365.Application.Abstractions.Repositories;

public interface IAneksRepository
{
    IAsyncEnumerable<AnekForViewing> GetNewAneks(AneksNewQuery query, CancellationToken cancellationToken);

    IAsyncEnumerable<AnekForViewing> GetPopularAneks(AneksPeriodedQuery query, CancellationToken cancellationToken);

    IAsyncEnumerable<AnekForViewing> GetMoreViewedAneks(AneksPeriodedQuery query, CancellationToken cancellationToken);

    IAsyncEnumerable<AnekDot> QueryAsync(AneksQuery query, CancellationToken cancellationToken);

    Task<int?> AddAnek(CreateAnekQuery query, CancellationToken cancellationToken);

    IAsyncEnumerable<AnekForViewing> GetNewWithTagIdAneks(
        AneksWithTagIdQuery query,
        CancellationToken cancellationToken);

    IAsyncEnumerable<AnekForViewing> GetPopularWithTagIdAneks(
        AneksWithTagIdQuery query,
        CancellationToken cancellationToken);

    IAsyncEnumerable<AnekForViewing> GetMoreViewedWithTagIdAneks(
        AneksWithTagIdQuery query,
        CancellationToken cancellationToken);

    IAsyncEnumerable<AnekForViewing> GetNewWithUserIdAneks(
        AneksWithUserIdQuery query,
        CancellationToken cancellationToken);

    IAsyncEnumerable<AnekForViewing> GetPopularWithUserIdAneks(
        AneksWithUserIdQuery query,
        CancellationToken cancellationToken);

    IAsyncEnumerable<AnekForViewing> GetMoreViewedWithUserIdAneks(
        AneksWithUserIdQuery query,
        CancellationToken cancellationToken);

    Task<int> CountInPeriod(DateTimeOffset border, CancellationToken cancellationToken);

    Task<int> CountByTagId(int tagId, CancellationToken cancellationToken);

    Task<int> CountByUserId(int userId, CancellationToken cancellationToken);

    Task AddViewing(int anekId, CancellationToken cancellationToken);
}