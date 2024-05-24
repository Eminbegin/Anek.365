using Anek._365.Application.Contracts.Services.Aneks;
using Anek._365.Application.Contracts.Services.Tags;
using Anek._365.Application.Contracts.Services.Users;

namespace Anek._365.Application.Contracts;

public interface IFacadeController
{
    IAneksService AneksService { get; }

    ITagsService TagsService { get; }

    IUserService UserService { get; }
}