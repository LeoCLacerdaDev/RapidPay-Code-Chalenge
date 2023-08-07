using System.Net;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RpDataHelper.Exceptions;

namespace RpDataHelper.Middlewares;

public class RpExceptionHandlerMiddleware : IMiddleware
{
    // private readonly RequestDelegate _next;
    //
    // public RpExceptionHandlerMiddleware(RequestDelegate next) =>
    //     _next = next;
    //
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (CustomException e)
        {
            await HandleExceptionAsync(context, e, e.StatusCode);
        }
        catch (Exception e)
        {
            await HandleExceptionAsync(context, e);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context, 
        Exception e, 
        HttpStatusCode code = HttpStatusCode.BadRequest)
    {
        var response = new
        {
            error = e.Message
        };
        context.Response.StatusCode = (int)code;
        await context.Response.WriteAsJsonAsync(response);
    }
}