using Anek._365.Application.Models;

namespace Anek._365.Presentation.Controllers.Creating;

public record CreatingAnekdotViewModel(
    string? Title,
    string? Content,
    Tag[] Tags,
    User? User) : IHaveUser
{
    public Tag[] Tags { get; } = Tags;

    public User? User { get; } = User;
}