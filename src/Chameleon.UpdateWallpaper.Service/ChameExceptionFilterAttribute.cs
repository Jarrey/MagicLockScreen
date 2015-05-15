using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using log4net;
using Chameleon.UpdateWallpaper.Service.Properties;

namespace Chameleon.UpdateWallpaper.Service
{
    /// <summary>Root exception handling filter for images. All exceptions will filter through here.</summary>
    public class ChameExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILog logger;

        /// <summary>Initializes a new instance of the <see cref="ImagesExceptionFilterAttribute"/> class.</summary>
        public ChameExceptionFilterAttribute()
        {
            this.logger = LogManager.GetLogger("Web Service");
        }

        /// <summary>
        /// Handler for exceptions.
        /// </summary> <param name="context"> The context.
        /// </param>
        public override void OnException(HttpActionExecutedContext context)
        {
            // Sanitise the exception, report to the client and log in detail.
            Exception exception = context.Exception;
            if (exception != null)
            {
                LogException(exception);

                // See if a reason for the exception was provided.
                string reason = GetExceptionMessage(exception);
                reason = reason.Replace(Environment.NewLine, " ");

                // Provide a sanitised user visible message. As the service develops, we will want to provide
                // more specific, user-friendly messages to the user and possibly the option to return.
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent(Resources.Http500ErrorString),
                    ReasonPhrase = reason
                };

                throw new HttpResponseException(response);
            }
        }

        /// <summary>
        /// Write this exception to the log.
        /// </summary> <param name="exception"> The exception.
        /// </param>
        private void LogException(Exception exception)
        {
            this.logger.Error("An exception occured:", exception);
        }

        /// <summary>
        /// Access the detailed message information from the exception, recursing into inner
        /// exceptions if necessary.
        /// </summary> 
        /// <param name="exception"> The exception. </param>
        private string GetExceptionMessage(Exception exception)
        {
            // Get the initial message.
            string message = exception.Message;

            // If we have inner exceptions then recurse until we get to the root.
            Exception innerException = exception.InnerException;
            while (innerException != null)
            {
                message += " --> " + innerException.Message;
                innerException = innerException.InnerException;
            }

            return message;
        }
    }
}
