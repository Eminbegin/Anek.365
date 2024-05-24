namespace Anek._365.Presentation.Controllers;

public interface IHavePaging
{
    int PageNumber { get; }

    string Controller { get; }

    string Action { get; }

    int MaxPage { get; }
}