using Anek._365.Application.Abstractions;
using Anek._365.Application.Contracts.Commands.Tags;
using Anek._365.Application.Contracts.Services.Tags;
using Anek._365.Application.Models;

namespace Anek._365.Application.Services;

public class TagsService : ITagsService
{
    private readonly IPersistenceContext _persistenceContext;

    public TagsService(IPersistenceContext persistenceContext)
    {
        _persistenceContext = persistenceContext;
    }

    public async Task<GetAllTagsCommand.Response> GetAllTagsAsync(
        GetAllTagsCommand.Request request,
        CancellationToken cancellationToken)
    {
        Tag[] result = await _persistenceContext.TagsRepository
            .GetAll(cancellationToken)
            .ToArrayAsync(cancellationToken);

        return new GetAllTagsCommand.Response(result);
    }

    public async Task<GetAllTagsWithViewsCommand.Response> GetAllTagsWithViewsAsync(
        GetAllTagsWithViewsCommand.Request request,
        CancellationToken cancellationToken)
    {
        TagWithCount[] result = await _persistenceContext.TagsRepository
            .GetAllWithViews(cancellationToken)
            .ToArrayAsync(cancellationToken);

        return new GetAllTagsWithViewsCommand.Response(result);
    }
}