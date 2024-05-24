using System.ComponentModel.DataAnnotations;

namespace Anek._365.Presentation.Controllers.Login;

public record LoginModel(
    [Required] string Name,
    [Required] string Password);