using System.ComponentModel.DataAnnotations;

namespace Anek._365.Presentation.Controllers.Login;

public record SignUpModel(
    [Required] string NickName,
    [Required] string Email,
    [Required] string Password);