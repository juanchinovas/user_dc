using Application.Common.Models;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Application
{
    public class CustomRequestMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomRequestMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            } catch (Exception ex)
            {
                context.Response.StatusCode = 409;
                context.Response.ContentType = "application/json";

                if (ex is not AppException)
                {
                    await context.Response.WriteAsync(
                        JsonSerializer.Serialize(
                            AppResponse<AppException>.Fail(
                                new AppException(ex.Message))));
                    return;
                }

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(
                        AppResponse<AppException>.Fail((AppException) ex)));
            }
        }
    }
}
