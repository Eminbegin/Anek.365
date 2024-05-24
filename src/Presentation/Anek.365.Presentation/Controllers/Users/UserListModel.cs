using Anek._365.Application.Models;

namespace Anek._365.Presentation.Controllers.Users;

public record UserListModel(
    UserWithCount[] Users,
    int PageNumber,
    string Controller,
    string Action,
    int MaxPage,
    User? User) : IHavePaging, IHaveUser
{
    public int PageNumber { get; } = PageNumber;

    public string Controller { get; } = Controller;

    public string Action { get; } = Action;

    public int MaxPage { get; } = MaxPage;

    public User? User { get; } = User;
}