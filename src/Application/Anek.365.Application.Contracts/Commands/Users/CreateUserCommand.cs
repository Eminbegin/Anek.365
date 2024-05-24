namespace Anek._365.Application.Contracts.Commands.Users;

public static class CreateUserCommand
{
    public sealed record Request(string Email, string NickName, string Password);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(int Id) : Response;

        public sealed record Failure(string Message) : Response;
    }
}