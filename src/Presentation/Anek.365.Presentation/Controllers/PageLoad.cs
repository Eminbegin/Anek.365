using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace Anek._365.Presentation.Controllers;

[AttributeUsage(AttributeTargets.Class)]
public sealed class PageLoad : ActionFilterAttribute
{
    private readonly Stopwatch _sw;

    public PageLoad()
    {
        _sw = new Stopwatch();
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        _sw.Start();
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        _sw.Stop();
        long ms = _sw.ElapsedMilliseconds;

        if (context.Result is ViewResult)
        {
            ((ViewResult)context.Result).ViewData["time"] = $"{ms} ms";
        }
    }
}