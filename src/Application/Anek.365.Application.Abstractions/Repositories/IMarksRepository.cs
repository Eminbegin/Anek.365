using Anek._365.Application.Abstractions.Marks;
using Anek._365.Application.Models;

namespace Anek._365.Application.Abstractions.Repositories;

public interface IMarksRepository
{
    Task AddMark(MarkQuery query, CancellationToken cancellationToken);

    Task AddView(MarkQuery query, CancellationToken cancellationToken);

    Task<Mark?> GetMark(int anekId, int userId, CancellationToken cancellationToken);
}