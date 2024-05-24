using Anek._365.Application.Models;

namespace Anek._365.Presentation.Controllers.Home;

public record HomeViewModel(
    AnekForViewing[] Aneks,
    string Title,
    string Action,
    string Controller,
    string Period,
    int PageNumber,
    (bool, bool, bool) Buttons,
    int MaxPage,
    User? User,
    string? ErrorMessage = null) : IHaveAneks, IHavePaging, IHaveUser
{
    public string Title { get; } = Title;

    public string Action { get; } = Action;

    public string Controller { get; } = Controller;

    public int MaxPage { get; } = MaxPage;

    public int PageNumber { get; } = PageNumber;

    public (bool New, bool Popular, bool MoreViewed) Buttons { get; } = Buttons;

    public User? User { get; } = User;

    public string? ErrorMessage { get; } = ErrorMessage;
}