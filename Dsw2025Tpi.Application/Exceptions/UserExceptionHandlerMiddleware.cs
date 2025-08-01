using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Exceptions
{
    public class UserExceptionHandlerMiddleware : AbstractExceptionHandlerMiddleware
    {
        public UserExceptionHandlerMiddleware(RequestDelegate next, ILogger<AbstractExceptionHandlerMiddleware> logger) : base(next, logger)
        {

        }
        public record SimpleResponse(string Message);

        public override (HttpStatusCode code, string message) GetResponse(Exception exception)
        {
            HttpStatusCode code;
            switch (exception)
            {
                case KeyNotFoundException
                     or NotSupportedException
                     or NotFoundException
                     or FileNotFoundException:
                    code = HttpStatusCode.NotFound;
                    break;
                case DuplicatedEntityException:
                    code = HttpStatusCode.Conflict;
                    break;
                case UnauthorizedAccessException:
                    code = HttpStatusCode.Unauthorized;
                    break;
                case InvalidOperationException
                     or ArgumentException:
                    code = HttpStatusCode.BadRequest;
                    break;
                default:
                    code = HttpStatusCode.InternalServerError;
                    break;
            }
            var message = exception.Message ?? "Undefined error";
            return (code, JsonSerializer.Serialize(new SimpleResponse(message)));
        }
    }
}
