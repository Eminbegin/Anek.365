using Anek._365.Application.Abstractions;
using Anek._365.Application.Abstractions.Marks;
using Anek._365.Application.Abstractions.Queries;
using Anek._365.Application.Abstractions.Repositories;
using Anek._365.Application.Contracts.Commands.Users;
using Anek._365.Application.Contracts.Services.Users;
using Anek._365.Application.Models;

namespace Anek._365.Application.Services;

public class UserService : IUserService
{
    private readonly IPersistenceContext _persistenceContext;
    private readonly IAuthenticationRepository _authenticationRepository;

    public UserService(IPersistenceContext persistenceContext, IAuthenticationRepository authenticationRepository)
    {
        _persistenceContext = persistenceContext;
        _authenticationRepository = authenticationRepository;
    }

    public async Task<GetUsersCommand.Response> GetUsersAsync(
        GetUsersCommand.Request request,
        CancellationToken cancellationToken)
    {
        UserWithCount[] users = await _persistenceContext.UsersRepository
            .GetWithViews(
                new(request.PageNumber, request.PageSize),
                cancellationToken)
            .ToArrayAsync(cancellationToken);

        int count = await _persistenceContext.UsersRepository.UsersCount(cancellationToken);

        return new GetUsersCommand.Response(users, count);
    }

    public async Task<CreateUserCommand.Response> CreateUserAsync(
        CreateUserCommand.Request request,
        CancellationToken cancellationToken)
    {
        if (request.Password.Length < 6)
        {
            return new CreateUserCommand.Response.Failure("The minimum password length is 6");
        }

        if (!request.NickName.All(CheckNickName))
        {
            return new CreateUserCommand.Response.Failure("NickName must consist of letters, digits and _");
        }

        User? userByName = await _persistenceContext.UsersRepository.GetByName(request.NickName, cancellationToken);

        User? userByStandardName = await _persistenceContext.UsersRepository
            .GetByStandardName(request.NickName, cancellationToken);

        if (userByName is not null || userByStandardName is not null)
        {
            return new CreateUserCommand.Response.Failure("User with this NickName already exists");
        }

        User? userByEmail = await _persistenceContext.UsersRepository.GetByName(request.NickName, cancellationToken);

        if (userByEmail is not null)
        {
            return new CreateUserCommand.Response.Failure("User with this Email already exists");
        }

        Register.Result register = await _authenticationRepository
            .Register(request.Email, request.Password, cancellationToken);

        if (register is Register.Result.Success)
        {
            int? userId = await _persistenceContext.UsersRepository
                .AddUser(new(request.NickName, request.Email), cancellationToken);

            if (userId is not null)
                return new CreateUserCommand.Response.Success(userId.Value);
        }

        return new CreateUserCommand.Response.Failure("Something wrong with firebase");
    }

    public async Task<AuthorizeUserCommand.Response> AuthorizeUserAsync(
        AuthorizeUserCommand.Request.ByNickName request,
        CancellationToken cancellationToken)
    {
        User? userByName = await _persistenceContext.UsersRepository.GetByName(request.NickName, cancellationToken);

        if (userByName is null)
            return new AuthorizeUserCommand.Response.Failure("Неверное имя пользователя");

        string? token = await _authenticationRepository
            .Login(userByName.Email, request.Password, cancellationToken);

        return token is null
            ? new AuthorizeUserCommand.Response.Failure("Неверный пароль")
            : new AuthorizeUserCommand.Response.Success(token);
    }

    public async Task<AuthorizeUserCommand.Response> AuthorizeUserAsync(
        AuthorizeUserCommand.Request.ByEmail request,
        CancellationToken cancellationToken)
    {
        string? token = await _authenticationRepository
            .Login(request.Email, request.Password, cancellationToken);

        return token is null
            ? new AuthorizeUserCommand.Response.Failure("Неверный пароль или почта")
            : new AuthorizeUserCommand.Response.Success(token);
    }

    public async Task<AddViewUserCommand.Response> AddViewAsync(
        AddViewUserCommand.Request request,
        CancellationToken cancellationToken)
    {
        Mark? mark = await _persistenceContext.MarksRepository
            .GetMark(request.AnekId, request.UserId, cancellationToken);

        if (mark is null)
        {
            await _persistenceContext.MarksRepository
                .AddView(
                    new MarkQuery(request.AnekId, request.UserId, 0),
                    cancellationToken);

            await _persistenceContext.AneksRepository.AddViewing(request.AnekId, cancellationToken);
        }

        return new AddViewUserCommand.Response();
    }

    public async Task<RateAnekCommand.Response> RateAsync(
        RateAnekCommand.Request request,
        CancellationToken cancellationToken)
    {
        await _persistenceContext.MarksRepository.AddMark(
            new MarkQuery(request.AnekId, request.UserId, request.Value),
            cancellationToken);

        return new RateAnekCommand.Response();
    }

    private bool CheckNickName(char c)
    {
        return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || char.IsDigit(c) || c == '_';
    }
}