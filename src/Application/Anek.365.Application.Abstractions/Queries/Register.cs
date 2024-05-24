namespace Anek._365.Application.Abstractions.Queries;

public static class Register
{
    public abstract record Result
    {
        private Result() { }

        public sealed record Failure(string Message) : Result;

        public sealed record Success() : Result;
    }
}