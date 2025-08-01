using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Exceptions
{
    public abstract class AbstractExceptionHandlerMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<AbstractExceptionHandlerMiddleware> _logger;

        public abstract (HttpStatusCode code, string message) GetResponse(Exception exception);

        public AbstractExceptionHandlerMiddleware(RequestDelegate next, ILogger<AbstractExceptionHandlerMiddleware> logger)
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
                _logger.LogError(ex, "An error occurred while processing the request.");
                var response = context.Response;
                context.Response.ContentType = "application/json";

                var (status, message) = GetResponse(ex);
                response.StatusCode = (int)status;
                await response.WriteAsync(message);
            }
        }
     }
}
