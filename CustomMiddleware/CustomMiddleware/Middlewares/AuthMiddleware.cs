using System.Reflection;
using CustomMiddleware.Attributes;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace CustomMiddleware.Middlewares;

public sealed class AuthMiddleware
{
    private readonly RequestDelegate _next;
    
    public AuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task<Task> Invoke(HttpContext context)
    {
        try
        {
            var endpoint = context.GetEndpoint();
            if (endpoint == null)
            {
                await _next(context);
                return Task.CompletedTask;
            }
            
            var actionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();

            var attributes = actionDescriptor!.MethodInfo.GetCustomAttributes();
            if (attributes.Any(x => x is MyCustomAllowAnonymousAttribute))
            {
                return _next(context);
            }
            
            // logic operation ...
            
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("You don't have permission to access this page");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
        return Task.CompletedTask;
    }
}
