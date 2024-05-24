namespace Anek._365.Application.Contracts.Commands.Users;

public static class AddViewUserCommand
{
    public sealed record Request(int AnekId, int UserId);

    public sealed record Response();
}