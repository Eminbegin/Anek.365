namespace Anek._365.Application.Contracts.Commands.Aneks;

public static class CreateAnekCommand
{
    public sealed record Request(int UserId, string Title, string Content, IEnumerable<int> Tags);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(int AnekId) : Response;

        public sealed record Failure : Response;
    }
}