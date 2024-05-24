using Anek._365.Application.Abstractions.Queries;

namespace Anek._365.Application.Abstractions.Repositories;

public interface IAuthenticationRepository
{
    Task<Register.Result> Register(string email, string password, CancellationToken cancellationToken);

    Task<string?> Login(string email, string password, CancellationToken cancellationToken);
}