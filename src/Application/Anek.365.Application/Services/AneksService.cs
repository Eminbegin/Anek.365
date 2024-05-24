using Anek._365.Application.Abstractions;
using Anek._365.Application.Abstractions.Queries.Aneks;
using Anek._365.Application.Abstractions.Queries.Tags;
using Anek._365.Application.Contracts.Commands.Aneks;
using Anek._365.Application.Contracts.Services.Aneks;
using Anek._365.Application.Extensions;
using Anek._365.Application.Models;
using Anek._365.Application.Tools;

namespace Anek._365.Application.Services;

public class AneksService : IAneksService
{
    private readonly IPersistenceContext _persistenceContext;

    public AneksService(IPersistenceContext persistenceContext)
    {
        _persistenceContext = persistenceContext;
    }

    public async Task<CreateAnekCommand.Response> CreateAnekAsync(
        CreateAnekCommand.Request request,
        CancellationToken cancellationToken)
    {
        int? result = await _persistenceContext.AneksRepository
            .AddAnek(
                CreateAnekQuery
                    .Build(x
                        => x.WithUserId(request.UserId)
                            .WithTitle(request.Title)
                            .WithContent(request.Content)
                            .WithCreatedAt(Calendar.CurrentDateTimeOffset())),
                cancellationToken);

        if (result is null)
        {
            return new CreateAnekCommand.Response.Failure();
        }

        await _persistenceContext.TagsRepository
            .AddAnek(
                AddAnekQuery.Build(x
                    => x.WithAnekId(result.Value)
                        .WithTagIds(request.Tags)),
                cancellationToken);

        return new CreateAnekCommand.Response.Success(result.Value);
    }

    public async Task<GetAnekListCommand.Response> GetAneksAsync(
        GetAnekListCommand.Request request,
        CancellationToken cancellationToken)
    {
        List<AnekDot> aneks = await _persistenceContext.AneksRepository.QueryAsync(
            AneksQuery.Build(x => x.WithIds(request.Ids)),
            cancellationToken).ToListAsync(cancellationToken);

        return new GetAnekListCommand.Response(aneks.ToArray());
    }

    public async Task<AddViewCommand.Response> AddViewAsync(
        AddViewCommand.Request request,
        CancellationToken cancellationToken)
    {
        await _persistenceContext.AneksRepository.AddViewing(request.AnekId, cancellationToken);
        return new AddViewCommand.Response();
    }

    public async Task<GetAneksCommand.Response> GetNewAneksAsync(
        GetAneksCommand.Request.News request,
        CancellationToken cancellationToken)
    {
        var border = Period.None.ToDateTimeOffset();

        int count = await _persistenceContext.AneksRepository
            .CountInPeriod(border, cancellationToken);

        AnekForViewing[] aneks = await _persistenceContext.AneksRepository
            .GetNewAneks(
                new AneksNewQuery(request.PageNumber, request.PageSize),
                cancellationToken)
            .ToArrayAsync(cancellationToken);

        return new GetAneksCommand.Response.Success(aneks, count, string.Empty);
    }

    public async Task<GetAneksCommand.Response> GetPopularAneksAsync(
        GetAneksCommand.Request.Popular request,
        CancellationToken cancellationToken)
    {
        var border = request.Period.ToDateTimeOffset();

        int count = await _persistenceContext.AneksRepository
            .CountInPeriod(border, cancellationToken);

        AnekForViewing[] aneks = await _persistenceContext.AneksRepository
            .GetPopularAneks(
                new AneksPeriodedQuery(border, request.PageNumber, request.PageSize),
                cancellationToken)
            .ToArrayAsync(cancellationToken);

        return new GetAneksCommand.Response.Success(aneks, count, string.Empty);
    }

    public async Task<GetAneksCommand.Response> GetMoreViewedAneksAsync(
        GetAneksCommand.Request.MoreViewed request,
        CancellationToken cancellationToken)
    {
        var border = request.Period.ToDateTimeOffset();

        int count = await _persistenceContext.AneksRepository
            .CountInPeriod(border, cancellationToken);

        AnekForViewing[] aneks = await _persistenceContext.AneksRepository
            .GetMoreViewedAneks(
                new AneksPeriodedQuery(border, request.PageNumber, request.PageSize),
                cancellationToken)
            .ToArrayAsync(cancellationToken);

        return new GetAneksCommand.Response.Success(aneks, count, string.Empty);
    }

    public async Task<GetOneAnekCommand.Response> GetAnekAsync(
        GetOneAnekCommand.Request request,
        CancellationToken cancellationToken)
    {
        AnekDot anek = await _persistenceContext.AneksRepository.QueryAsync(
                AneksQuery.Build(x => x.WithId(request.Id)),
                cancellationToken)
            .FirstAsync(cancellationToken);

        User? user = await _persistenceContext.UsersRepository.GetById(anek.UserId, cancellationToken);

        if (user is null)
        {
            return new GetOneAnekCommand.Response.Failure("No User");
        }

        Tag[] tags = await _persistenceContext.TagsRepository
            .GetByAnekId(request.Id, cancellationToken)
            .ToArrayAsync(cancellationToken);

        return new GetOneAnekCommand.Response.Success(anek, user, tags);
    }

    public async Task<GetAneksCommand.Response> GetNewAneksByTagAsync(
        GetAneksCommand.Request.NewByTag request,
        CancellationToken cancellationToken)
    {
        Tag? tag = await _persistenceContext.TagsRepository.GetByStandardName(request.Name, cancellationToken);

        if (tag is null)
        {
            return new GetAneksCommand.Response.Failure();
        }

        int count = await _persistenceContext.AneksRepository
            .CountByTagId(tag.Id, cancellationToken);

        AnekForViewing[] aneks = await _persistenceContext.AneksRepository
            .GetNewWithTagIdAneks(
                new AneksWithTagIdQuery(tag.Id, request.PageNumber, request.PageSize),
                cancellationToken)
            .ToArrayAsync(cancellationToken);

        return new GetAneksCommand.Response.Success(aneks, count, tag.Name);
    }

    public async Task<GetAneksCommand.Response> GetPopularAneksByTagAsync(
        GetAneksCommand.Request.PopularByTag request,
        CancellationToken cancellationToken)
    {
        Tag? tag = await _persistenceContext.TagsRepository.GetByStandardName(request.Name, cancellationToken);

        if (tag is null)
        {
            return new GetAneksCommand.Response.Failure();
        }

        int count = await _persistenceContext.AneksRepository
            .CountByTagId(tag.Id, cancellationToken);

        AnekForViewing[] aneks = await _persistenceContext.AneksRepository
            .GetPopularWithTagIdAneks(
                new AneksWithTagIdQuery(tag.Id, request.PageNumber, request.PageSize),
                cancellationToken)
            .ToArrayAsync(cancellationToken);

        return new GetAneksCommand.Response.Success(aneks, count, tag.Name);
    }

    public async Task<GetAneksCommand.Response> GetMoreViewedAneksByTagAsync(
        GetAneksCommand.Request.MoreViewedByTag request,
        CancellationToken cancellationToken)
    {
        Tag? tag = await _persistenceContext.TagsRepository.GetByStandardName(request.Name, cancellationToken);

        if (tag is null)
        {
            return new GetAneksCommand.Response.Failure();
        }

        int count = await _persistenceContext.AneksRepository
            .CountByTagId(tag.Id, cancellationToken);

        AnekForViewing[] aneks = await _persistenceContext.AneksRepository
            .GetMoreViewedWithTagIdAneks(
                new AneksWithTagIdQuery(tag.Id, request.PageNumber, request.PageSize),
                cancellationToken)
            .ToArrayAsync(cancellationToken);

        return new GetAneksCommand.Response.Success(aneks, count, tag.Name);
    }

    public async Task<GetAneksCommand.Response> GetNewAneksByUserAsync(
        GetAneksCommand.Request.NewByUser request,
        CancellationToken cancellationToken)
    {
        User? user = await _persistenceContext.UsersRepository.GetByStandardName(request.Name, cancellationToken);

        if (user is null)
        {
            return new GetAneksCommand.Response.Failure();
        }

        int count = await _persistenceContext.AneksRepository
            .CountByUserId(user.Id, cancellationToken);

        AnekForViewing[] aneks = await _persistenceContext.AneksRepository
            .GetNewWithUserIdAneks(
                new AneksWithUserIdQuery(user.Id, request.PageNumber, request.PageSize),
                cancellationToken)
            .ToArrayAsync(cancellationToken);

        return new GetAneksCommand.Response.Success(aneks, count, user.Name);
    }

    public async Task<GetAneksCommand.Response> GetPopularAneksByUserAsync(
        GetAneksCommand.Request.PopularByUser request,
        CancellationToken cancellationToken)
    {
        User? user = await _persistenceContext.UsersRepository.GetByStandardName(request.Name, cancellationToken);

        if (user is null)
        {
            return new GetAneksCommand.Response.Failure();
        }

        int count = await _persistenceContext.AneksRepository
            .CountByUserId(user.Id, cancellationToken);

        AnekForViewing[] aneks = await _persistenceContext.AneksRepository
            .GetPopularWithUserIdAneks(
                new AneksWithUserIdQuery(user.Id, request.PageNumber, request.PageSize),
                cancellationToken)
            .ToArrayAsync(cancellationToken);

        return new GetAneksCommand.Response.Success(aneks, count, user.Name);
    }

    public async Task<GetAneksCommand.Response> GetMoreViewedAneksByUserAsync(
        GetAneksCommand.Request.MoreViewedByUser request,
        CancellationToken cancellationToken)
    {
        User? user = await _persistenceContext.UsersRepository.GetByStandardName(request.Name, cancellationToken);

        if (user is null)
        {
            return new GetAneksCommand.Response.Failure();
        }

        int count = await _persistenceContext.AneksRepository
            .CountByUserId(user.Id, cancellationToken);

        AnekForViewing[] aneks = await _persistenceContext.AneksRepository
            .GetMoreViewedWithUserIdAneks(
                new AneksWithUserIdQuery(user.Id, request.PageNumber, request.PageSize),
                cancellationToken)
            .ToArrayAsync(cancellationToken);

        return new GetAneksCommand.Response.Success(aneks, count, user.Name);
    }
}