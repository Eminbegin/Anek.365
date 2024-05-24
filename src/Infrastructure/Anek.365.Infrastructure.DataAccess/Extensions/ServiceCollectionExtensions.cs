using Anek._365.Application.Abstractions;
using Anek._365.Application.Abstractions.Repositories;
using Anek._365.Infrastructure.Authentication;
using Anek._365.Infrastructure.DataAccess;
using Anek._365.Infrastructure.DataAccess.Migrations;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Itmo.Dev.Platform.Common.Extensions;
using Itmo.Dev.Platform.Postgres.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Itmo.Dev.Asap.Points.Infrastructure.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection collection)
    {
        collection.AddPlatform();
        collection.AddPlatformPostgres(builder => builder.BindConfiguration("Infrastructure:Postgres"));

        collection.AddScoped<IPersistenceContext, PersistenceContext>();
        collection.AddScoped<IAneksRepository, AneksRepository>();
        collection.AddScoped<ITagsRepository, TagsRepository>();
        collection.AddScoped<IUsersRepository, UsersRepository>();
        collection.AddScoped<IMarksRepository, MarksRepository>();

        collection.AddPlatformMigrations(typeof(IAssemblyMarker).Assembly);

        collection.AddHostedService<MigrationRunnerService>();
        return collection;
    }

    public static IServiceCollection AddAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string json =
            """{ "type": "service_account", "project_id": "anek-365", "private_key_id": "4a0c0431d1074fc0f11cd1a6fc533665348b7581", "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCgHs0nUXM6Gy1T\nmgF6C0ossenzJJY5yIEnqhB0bOttF6VKsvJ2ZKmVAEEfBo0xT4w9uwA1LZRSaLgs\nGZuvg/AJO1i0NDIShdauFPijDBqmkyZEbbn883Rl+ctoUxaQw0pCGSLAOrzMR4U4\ny21cWPGEKIpRBhSKTAslxL73nbukgUg0V7RHQl2YOUz+R3nRw4zww1neGTYaiivJ\nC2YTn5lo+eqxxca5woy3F32dIv2r7HySJD9L+PXxk85nCtk0yY+neeGDEUhZwsAx\nj1cfFMzK+ovw9Lv/x0H85J1vRSc+sIkYhfYb+4GcQN4IJzOGpGeF+Bo9vwc+X/BI\nEwRnYmHnAgMBAAECggEADGLFkNSKmsSOaYigs52a88EhbTb3zel2ICjWFAN25J3v\nIcuc2M6plYaLAwjhNB7IpN/JwNNRRVynXnXuppGIqFYoYuwksW0b8aRapVYi257H\nV4S7XddBOkAr3VqMBM+ud1V1bDQ+9XhU1FNcFsgkiooORP046oe+VwIUmZfgc3WT\nWte+ys3rzFMrcGnC40Qx8OzZzGzA3PSoq+4Y4GeK/BOKGcgRKddKR8fWHTrYi+lq\nl1nhiBt2DP4NZplToy0mhRzbCuMMRJPR3d0OhR/s2MWdCdshXtD7aYY13ZcO/dAb\nLJH6dAZa3i0I83eWPhxxA0rIJlK1FNHaOm2R4IX2aQKBgQDMTz/Dyh1Hm2ZkcjVW\nShmUIlIU1NBELCARsyiliv8SGwsKF5Sss1iPre2RyP8Q9vRdNXtZ9COTfgrNsa7O\n7XgfjSnV+aegUVd4olH1MrrmaRJSkFdvJGHGoEY6E1H6XdfpSkvjg7K9ce84VlNF\n9EkGaov1vEHs6e6tQrKG2AXcCQKBgQDIoX8tCJWkE6aOyFr+CKHBZ2lhc+i01+Qm\nOc0y/M1M+R7Q+3FQEAlHSiYdKXV3DwJZJLwKYk/veDN25DxKSOTu0H2jEbpLQveX\nXMvLACYw+8BkBkpFG7aR/A9oi5lCZaA+aT+M7SF1fNzqHpLTTX5Xzb3TgGkCaurb\n1tpnFWqqbwKBgG4jFou0K96LLkJdstUGcw2BBuCF7JOjedIw02uSJ3iaLXROS+5h\nbA9gQl5BjQaXKhoJF6rhSkBbRPWnoEWajBPuJCxePffVgaLdVfMpWsmUwLTTbN7U\nLQVHJzRtb9bYuejgDVLjjvpDLiMfnhpq2ubjjvl0d9GANhMejmDUmSwJAoGBAIGO\nR+F8uaTKS7PIDXodeNa7QIF9p7ef8eP9T1StiqU5+XxZ4kIhGNzANPx/2SqYtpt5\n7TOHaLbql9EecRBHIg5U88xpljMje15yvebIoeDfsMzmgu89hhee/RQ1kTIfDes/\na5pFRmXny4AjXgy07fElGn+JXEYDZMWxaDdIzShhAoGAdu86s48NyS+dsmflEoJm\n4kDGbTv1LEC4rcznohQ5yrgo6IRlUmrVCUHZJ7vYFGKsRwG9orb/iQhe3vU84zkt\nVAWhitrM/8D1uy1oXoYmcUDGQO4Ozg0oEQuc7LtF6JJawWI1UJQoXikQq8bukwAh\nMLXQthfpSztfNnh6zW1O9aA=\n-----END PRIVATE KEY-----\n", "client_email": "firebase-adminsdk-o5jfi@anek-365.iam.gserviceaccount.com", "client_id": "107967638335607067722", "auth_uri": "https://accounts.google.com/o/oauth2/auth", "token_uri": "https://oauth2.googleapis.com/token", "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs", "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-o5jfi%40anek-365.iam.gserviceaccount.com", "universe_domain": "googleapis.com"}""";
        FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromJson(json),
        });

        services.AddSingleton<IAuthenticationRepository, AuthenticationRepository>();
        services.Configure<AuthenticationConfiguration>(options =>
            configuration.GetSection("Firebase:AuthenticationConfiguration").Bind(options));

        string projectName = configuration.GetSection("Firebase:ProjectName").Get<string>()!;

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"https://securetoken.google.com/{projectName}";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"https://securetoken.google.com/{projectName}",
                    ValidateAudience = true,
                    ValidAudience = projectName,
                    ValidateLifetime = true,
                };

                // options.SaveToken
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["AAAA"];
                        return Task.CompletedTask;
                    },
                };
            });

        return services;
    }
}