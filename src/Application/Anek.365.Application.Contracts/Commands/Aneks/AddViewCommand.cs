namespace Anek._365.Application.Contracts.Commands.Aneks;

public static class AddViewCommand
{
    public sealed record Request(int AnekId);

    public sealed record Response();
}