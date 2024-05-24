namespace Anek._365.Application.Contracts.Commands.Users;

public static class RateAnekCommand
{
    public sealed record Request(int AnekId, int UserId, int Value);

    public sealed record Response();
}