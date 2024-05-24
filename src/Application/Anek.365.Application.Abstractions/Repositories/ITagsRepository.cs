using Anek._365.Application.Abstractions.Queries.Tags;
using Anek._365.Application.Models;

namespace Anek._365.Application.Abstractions.Repositories;

public interface ITagsRepository
{
    IAsyncEnumerable<TagWithCount> GetAllWithViews(CancellationToken cancellationToken);

    IAsyncEnumerable<Tag> GetAll(CancellationToken cancellationToken);

    Task<Tag?> GetByStandardName(string standardName, CancellationToken cancellationToken);

    IAsyncEnumerable<Tag> GetByAnekId(int anekId, CancellationToken cancellationToken);

    Task AddAnek(AddAnekQuery query, CancellationToken cancellationToken);
}