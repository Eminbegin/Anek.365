using Anek._365.Application.Models;

namespace Anek._365.Application.Contracts.Commands.Aneks;

public static class GetAneksCommand
{
    public abstract record Request
    {
        private Request() { }

        public sealed record News(int PageNumber, int PageSize) : Request;

        public sealed record Popular(int PageNumber, int PageSize, Period Period) : Request;

        public sealed record MoreViewed(int PageNumber, int PageSize, Period Period) : Request;

        public sealed record NewByTag(string Name, int PageNumber, int PageSize) : Request;

        public sealed record PopularByTag(string Name, int PageNumber, int PageSize) : Request;

        public sealed record MoreViewedByTag(string Name, int PageNumber, int PageSize) : Request;

        public sealed record NewByUser(string Name, int PageNumber, int PageSize) : Request;

        public sealed record PopularByUser(string Name, int PageNumber, int PageSize) : Request;

        public sealed record MoreViewedByUser(string Name, int PageNumber, int PageSize) : Request;
    }

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(IEnumerable<AnekForViewing> AnekDots, int Count, string Name) : Response;

        public sealed record Failure() : Response;
    }
}