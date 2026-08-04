using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Exceptions;

namespace WebApi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                context.Response.StatusCode = 404;
                context.Response.ContentType = "application/json";

                var result = JsonSerializer.Serialize(new
                {
                    message = ex.Message,
                    statusCode = 404
                });

                await context.Response.WriteAsync(result);
            }
            catch (BadRequestException ex)
            {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";

                var result = JsonSerializer.Serialize(new
                {
                    message = ex.Message,
                    statusCode = 400
                });

                await context.Response.WriteAsync(result);
            }
            catch (InsufficientStockException ex)
            {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";

                var result = JsonSerializer.Serialize(new
                {
                    message = ex.Message,
                    statusCode = 400
                });

                await context.Response.WriteAsync(result);
            }
            catch (Exception)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var result = JsonSerializer.Serialize(new
                {
                    message = "An unexpected error occurred.",
                    statusCode = 500
                });

                await context.Response.WriteAsync(result);
            }
        }
    }
}
