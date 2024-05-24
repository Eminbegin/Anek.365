using Anek._365.Application.Models;

namespace Anek._365.Application.Contracts.Commands.Aneks;

public static class GetOneAnekCommand
{
    public sealed record Request(int Id);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(AnekDot AnekDot, User User, Tag[] Tags) : Response;

        public sealed record Failure(string Message) : Response;
    }
}