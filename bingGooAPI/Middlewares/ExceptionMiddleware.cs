using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace bingGooAPI.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
              
                await _next(context);
            }
            catch (Exception ex)
            {
         
                _logger.LogError(ex, ex.Message);

          
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(
      HttpContext context,
      Exception exception)
        {
            context.Response.ContentType = "application/json";

            int statusCode;
            string message;

            switch (exception)
            {
                case SqlException sqlEx when sqlEx.Number == 547:
                    statusCode = StatusCodes.Status400BadRequest;
                    message = "Cannot delete this supplier because it is being used by Create Product.";
                    break;

                case UnauthorizedAccessException:
                    statusCode = StatusCodes.Status401Unauthorized;
                    message = exception.Message;
                    break;

                case KeyNotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    message = exception.Message;
                    break;

                case ArgumentException:
                    statusCode = StatusCodes.Status400BadRequest;
                    message = exception.Message;
                    break;

                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    message = exception.Message;
                    break;
            }

            context.Response.StatusCode = statusCode;

            var response = new
            {
                Success = false,
                StatusCode = statusCode,
                Message = message
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
