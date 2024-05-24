using Anek._365.Application.Contracts;
using Anek._365.Application.Contracts.Services.Aneks;
using Anek._365.Application.Contracts.Services.Tags;
using Anek._365.Application.Contracts.Services.Users;
using Anek._365.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Anek._365.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(
        this IServiceCollection collection)
    {
        collection.AddScoped<CurrentUserManager>();
        collection.AddScoped<IAneksService, AneksService>();
        collection.AddScoped<IUserService, UserService>();
        collection.AddScoped<ITagsService, TagsService>();
        collection.AddScoped<IFacadeController, FacadeController>();
        return collection;
    }
}