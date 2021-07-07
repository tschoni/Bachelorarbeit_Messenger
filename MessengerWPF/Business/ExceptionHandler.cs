using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace MessengerWPF.Business
{
    /// <summary>
    /// ExceptionHandler.
    /// </summary>
    public class ExceptionHandler
    {
        /// <summary>
        /// Gets the supported exceptions.
        /// </summary>
        /// <value>The supported exceptions.</value>
        public static Dictionary<HttpStatusCode, Type> SupportedExceptions { get; } = new Dictionary<HttpStatusCode, Type>()
        {
            { HttpStatusCode.NotFound, typeof(KeyNotFoundException) },
            { HttpStatusCode.Unauthorized, typeof(AuthenticationException) },
            { HttpStatusCode.BadRequest, typeof(ArgumentException) },
            { HttpStatusCode.InternalServerError, typeof(Exception) },
        };

        /// <summary>
        /// Handles the asynchronous.
        /// </summary>
        /// <param name="context">The context.</param>
        public async Task HandleAsync(HttpContext context)
        {
            var feature = context.Features.Get<IExceptionHandlerFeature>();

            var error = new
            {
                code = ExceptionHandler.SupportedExceptions.FirstOrDefault(x => x.Value.IsInstanceOfType(feature.Error)).Key,
                message = this.GetExceptionDetails(feature.Error),
            };

            context.Response.StatusCode = 500;
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "Unhandled Exception";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(error));
        }

        /// <summary>
        /// Gets the exception details.
        /// </summary>
        /// <param name="exception">The exception.</param>
        private string GetExceptionDetails(Exception exception)
        {
            string result = string.Empty;

            string stack = exception.StackTrace;

            while (exception != null)
            {
                result += exception.Message + Environment.NewLine;
                exception = exception.InnerException;
            }

            result += stack;

            return result;
        }
    }
}
