using Anek._365.Application.Abstractions;
using Anek._365.Application.Abstractions.Repositories;

namespace Anek._365.Infrastructure.DataAccess;

public class PersistenceContext : IPersistenceContext
{
    public PersistenceContext(
        IAneksRepository aneksRepository,
        IUsersRepository usersRepository,
        ITagsRepository tagsRepository,
        IMarksRepository marksRepository)
    {
        AneksRepository = aneksRepository;
        UsersRepository = usersRepository;
        TagsRepository = tagsRepository;
        MarksRepository = marksRepository;
    }

    public IAneksRepository AneksRepository { get; }

    public IUsersRepository UsersRepository { get; }

    public ITagsRepository TagsRepository { get; }

    public IMarksRepository MarksRepository { get; }

    // public IAuthenticationRepository AuthenticationRepository { get; }
}