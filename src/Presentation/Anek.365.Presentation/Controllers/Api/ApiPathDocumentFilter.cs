using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Anek._365.Presentation.Controllers.Api;

public class ApiPathDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var paths = swaggerDoc.Paths
            .Where(p => p.Key.StartsWith("/api"))
            .ToDictionary(p => p.Key, p => p.Value);

        swaggerDoc.Paths = new OpenApiPaths();
        foreach (KeyValuePair<string, OpenApiPathItem> path in paths)
        {
            swaggerDoc.Paths.Add(path.Key, path.Value);
        }
    }
}