using Anek._365.Application.Abstractions.Repositories;

namespace Anek._365.Application.Abstractions;

public interface IPersistenceContext
{
    IAneksRepository AneksRepository { get; }

    IUsersRepository UsersRepository { get; }

    ITagsRepository TagsRepository { get; }

    IMarksRepository MarksRepository { get; }

    // IAuthenticationRepository AuthenticationRepository { get; }
}