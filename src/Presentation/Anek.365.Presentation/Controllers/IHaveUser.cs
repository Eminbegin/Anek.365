using Anek._365.Application.Models;

namespace Anek._365.Presentation.Controllers;

public interface IHaveUser
{
    User? User { get; }
}