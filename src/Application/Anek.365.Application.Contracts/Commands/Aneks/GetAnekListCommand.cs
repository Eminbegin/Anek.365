using Anek._365.Application.Models;

namespace Anek._365.Application.Contracts.Commands.Aneks;

public static class GetAnekListCommand
{
    public sealed record Request(int[] Ids);

    public sealed record Response(AnekDot[] Aneks);
}