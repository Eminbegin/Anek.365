using Anek._365.Application.Models;

namespace Anek._365.Presentation.Controllers.Anekdots;

public record GetAnekdotViewModel(
    string Title,
    string Text,
    int AnekId,
    DateOnly Date,
    int Mark,
    int Views,
    Tag[] Tags,
    User Author,
    User? User) : IHaveUser
{
    public User? User { get; } = User;
}