namespace Anek._365.Presentation.Controllers.Login;

public record SignUpErrorModel(
    string? NickName = null,
    string? Email = null,
    string? Error = null);