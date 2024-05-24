using Anek._365.Application.Models;

namespace Anek._365.Application.Contracts.Commands.Tags;

public static class GetAllTagsWithViewsCommand
{
    public sealed record Request();

    public sealed record Response(IEnumerable<TagWithCount> Tags);
}