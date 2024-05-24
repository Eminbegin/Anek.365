using Anek._365.Application.Models;

namespace Anek._365.Presentation.Controllers.Users;

public record UserAneksModel(
    AnekForViewing[] Aneks,
    string Title,
    string Action,
    string Controller,
    string StandardName,
    int PageNumber,
    (bool New, bool Popular, bool MoreViewed) Buttons,
    int MaxPage,
    User? User) : IHaveAneks, IHavePaging, IHaveUser
{
    public string Title { get; } = Title;

    public string Action { get; } = Action;

    public string Controller { get; } = Controller;

    public string StandardName { get; } = StandardName;

    public int PageNumber { get; } = PageNumber;

    public int MaxPage { get; } = MaxPage;

    public (bool New, bool Popular, bool MoreViewed) Buttons { get; } = Buttons;

    public User? User { get; } = User;
}