using Anek._365.Application.Contracts.Commands.Tags;

namespace Anek._365.Application.Contracts.Services.Tags;

public interface ITagsService
{
    Task<GetAllTagsCommand.Response> GetAllTagsAsync(
        GetAllTagsCommand.Request request,
        CancellationToken cancellationToken);

    Task<GetAllTagsWithViewsCommand.Response> GetAllTagsWithViewsAsync(
        GetAllTagsWithViewsCommand.Request request,
        CancellationToken cancellationToken);
}