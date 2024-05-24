using Anek._365.Application.Models;

namespace Anek._365.Presentation.Controllers.Api.Creating;

public record AnekResponse(AnekDot AnekDot, User User, Tag[] Tags);