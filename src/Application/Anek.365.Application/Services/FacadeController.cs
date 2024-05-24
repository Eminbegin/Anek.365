using Anek._365.Application.Contracts;
using Anek._365.Application.Contracts.Services.Aneks;
using Anek._365.Application.Contracts.Services.Tags;
using Anek._365.Application.Contracts.Services.Users;

namespace Anek._365.Application.Services;

public class FacadeController : IFacadeController
{
    public FacadeController(IAneksService aneksService, ITagsService tagsService, IUserService userService)
    {
        AneksService = aneksService;
        TagsService = tagsService;
        UserService = userService;
    }

    public IAneksService AneksService { get; }

    public ITagsService TagsService { get; }

    public IUserService UserService { get; }
}