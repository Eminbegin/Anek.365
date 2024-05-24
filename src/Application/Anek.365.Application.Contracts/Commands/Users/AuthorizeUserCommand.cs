namespace Anek._365.Application.Contracts.Commands.Users;

public static class AuthorizeUserCommand
{
    public abstract record Request
    {
        private Request() { }

        public sealed record ByNickName(string NickName, string Password) : Request;

        public sealed record ByEmail(string Email, string Password) : Request;
    }

    public abstract record Response
    {
        private Response() { }

        public sealed record Failure(string Message) : Response;

        public sealed record Success(string Token) : Response;
    }
}