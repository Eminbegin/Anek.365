#pragma warning disable CA1506
using Anek._365.Application.Application;
using Anek._365.Application.Extensions;
using Anek._365.Presentation.Controllers.Api;
using Anek._365.Presentation.Hubs;
using Itmo.Dev.Asap.Points.Infrastructure.Persistence.Extensions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Anek._365.Presentation;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddAuthentication(builder.Configuration)
            .AddInfrastructure()
            .AddServices()
            .AddControllersWithViews();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddControllers();
        builder.Services.AddSignalR(options => { options.AddFilter<HubFilter>(); });

        builder.Services.AddEndpointsApiExplorer()
            .AddSwaggerGen(options =>
            {
                string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Anek.365", Version = "v1" });

                options.DocumentFilter<ApiPathDocumentFilter>();
                options.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Enter valid token",
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        BearerFormat = "JWT",
                        Scheme = "Bearer",
                    });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                        },
                        Array.Empty<string>()
                    },
                });
            });

        builder.Services.AddMvc();

        WebApplication app = builder.Build();

        // await using (AsyncServiceScope scope = app.Services.CreateAsyncScope())
        // {
        //     await scope.UseDataAccessAsync(default);
        // }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapControllers();
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=GetAnekdots}");
        app.UseMiddleware<AuthenticationMiddleware>();
        app.MapHub<RateHub>("/rate-anek");
        app.MapHub<AnekDataHub>("/page-data");
        app.MapHub<AnekHub>("/update-data");
        app.Run();
    }
}