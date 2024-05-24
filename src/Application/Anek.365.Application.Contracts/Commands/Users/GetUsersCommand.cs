using Anek._365.Application.Models;

namespace Anek._365.Application.Contracts.Commands.Users;

public static class GetUsersCommand
{
    public sealed record Request(int PageNumber, int PageSize);

    public sealed record Response(UserWithCount[] Users, int Count);
}