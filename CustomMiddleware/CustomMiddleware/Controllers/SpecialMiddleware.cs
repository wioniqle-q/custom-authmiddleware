using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CoronaVirusApi.Attributes;
using CoronaVirusApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace CoronaVirusApi.Middlewares
{
    public sealed class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        
        public AuthorizationMiddleware(RequestDelegate next)
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
                if (attributes.Any(x => x is SkipAuthAttribute))
                {
                    return _next(context);
                }
            
                var userService = (IUserService)context.RequestServices.GetService(typeof(IUserService));
                var authHeader = context.Request.Headers["Authorization"].SingleOrDefault();

                if (authHeader != null && userService.ValidateToken(authHeader))
                {
                    var needRole = context.GetEndpoint()!
                        .Metadata
                        .GetMetadata<NeedRoleAttribute>();

                    if (needRole == null || needRole.Role == userService.CurrentUser.Role)
                    {
                        return _next(context);
                    }
                }
            
                context.Response.StatusCode = 401;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        
            return Task.CompletedTask;
        }
    }
}
