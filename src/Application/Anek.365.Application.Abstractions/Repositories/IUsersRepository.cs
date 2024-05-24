using Anek._365.Application.Abstractions.Queries.Users;
using Anek._365.Application.Models;

namespace Anek._365.Application.Abstractions.Repositories;

public interface IUsersRepository
{
    IAsyncEnumerable<UserWithCount> GetWithViews(UsersQuery query, CancellationToken cancellationToken);

    Task<User?> GetById(int id, CancellationToken cancellationToken);

    Task<User?> GetByEmail(string email, CancellationToken cancellationToken);

    Task<User?> GetByName(string name, CancellationToken cancellationToken);

    Task<User?> GetByStandardName(string name, CancellationToken cancellationToken);

    Task<int?> AddUser(CreateUserQuery query, CancellationToken cancellationToken);

    Task<int> UsersCount(CancellationToken cancellationToken);
}