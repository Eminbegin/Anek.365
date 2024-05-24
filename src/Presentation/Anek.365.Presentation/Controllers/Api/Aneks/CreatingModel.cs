namespace Anek._365.Presentation.Controllers.Api.Creating;

public record CreatingModel(
    string Title,
    string Content,
    int[] TagIds);