namespace Anek._365.Application.Models;

public record AnekDot(
    int Id,
    string Title,
    string Text,
    DateTimeOffset CreatedAt,
    int Mark,
    int Views,
    int UserId);