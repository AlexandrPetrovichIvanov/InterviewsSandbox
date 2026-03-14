using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using YandexSandbox.Api.Mapping;
using YandexSandbox.Bll.Exceptions;

namespace YandexSandbox.Api;

public static class ExceptionHandler
{
    public static void UseBusinessExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(error => error.Run(async context =>
        {
            var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

            ProblemDetails problem;
            if (exception is BllException bllEx && BllExceptionMap.TryMap(bllEx, out var statusCode, out var message))
            {
                problem = new ProblemDetails
                {
                    Status = statusCode,
                    Detail = message
                };
            }
            else
            {
                problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An unexpected error occurred."
                };
            }

            context.Response.StatusCode = problem.Status!.Value;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }));
    }
}
