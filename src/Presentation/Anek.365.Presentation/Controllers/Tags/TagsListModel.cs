using Anek._365.Application.Models;

namespace Anek._365.Presentation.Controllers.Tags;

public record TagsListModel(TagWithCount[] TagsList, User? User) : IHaveUser
{
    public TagWithCount[] TagsList { get; } = TagsList;

    public User? User { get; } = User;
}