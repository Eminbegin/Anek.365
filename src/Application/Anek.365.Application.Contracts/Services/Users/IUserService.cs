using Anek._365.Application.Contracts.Commands.Users;

namespace Anek._365.Application.Contracts.Services.Users;

public interface IUserService
{
    Task<GetUsersCommand.Response> GetUsersAsync(
        GetUsersCommand.Request request,
        CancellationToken cancellationToken);

    Task<CreateUserCommand.Response> CreateUserAsync(
        CreateUserCommand.Request request,
        CancellationToken cancellationToken);

    Task<AuthorizeUserCommand.Response> AuthorizeUserAsync(
        AuthorizeUserCommand.Request.ByNickName request,
        CancellationToken cancellationToken);

    Task<AuthorizeUserCommand.Response> AuthorizeUserAsync(
        AuthorizeUserCommand.Request.ByEmail request,
        CancellationToken cancellationToken);

    Task<AddViewUserCommand.Response> AddViewAsync(
        AddViewUserCommand.Request request,
        CancellationToken cancellationToken);

    Task<RateAnekCommand.Response> RateAsync(
        RateAnekCommand.Request request,
        CancellationToken cancellationToken);
}