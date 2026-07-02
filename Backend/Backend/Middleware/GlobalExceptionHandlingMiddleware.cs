using Application.ExceptionLogging;
using Application.Exceptions;
using FluentValidation;

namespace ASPLAB2.API.Middleware
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IExceptionLogger _logger;
        public GlobalExceptionHandlingMiddleware(RequestDelegate next, IExceptionLogger logger)
        {
            _next = next;
            _logger = logger;
        }

        /*
            Zaduzenja global ex middleware-a
            1. Vrati response code na osnovu tipa izuzetka (mapiranje tipa izuzetka na status code)
            2. Logging izuzetka
       */
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (System.Exception ex)
            {
                context.Response.ContentType = "application/json";

                if (ex is ValidationException e) //StatusCode -> 422, Body sa greskama,
                {
                    context.Response.StatusCode = 422;
                    var errors = e.Errors.Select(x => new
                    {
                        error = x.ErrorMessage,
                        property = x.PropertyName
                    });

                    await context.Response.WriteAsJsonAsync(errors); // -> upisuje podatke u response body, ova operacija je asinhrona
                    return;
                }
                if (ex is LoginException loginException)
                {
                    // Exception za ne validne login kredencijale
                    context.Response.StatusCode = 400;



                    await context.Response.WriteAsJsonAsync(new { message = loginException.Message });
                    return;

                }

                if (ex is EntityNotFoundException EntityException)
                {
                    context.Response.StatusCode = 404;


                    await context.Response.WriteAsJsonAsync(new { message = EntityException.Message });

                    return;

                }
                if (ex is NotAuthorizedException NotAuthorized)
                {
                    context.Response.StatusCode = 401;


                    await context.Response.WriteAsJsonAsync(new { message = NotAuthorized.Message });

                    return;


                }


                //if (ex is UnauthorizedUseCaseException)
                //{
                //    context.Response.StatusCode = 401;
                //    return;
                //}

                context.Response.StatusCode = 500;

                //Guid id = new Guid();
                //DateTime timeOfError = DateTime.Now;
                //var logMessage = ex.Message;
                //var StackTrace = ex.StackTrace;

                //string FullLogMessage = $"id: {id}, time of error: {timeOfError}, error message: {logMessage}, stack trace: {StackTrace}";

                //_logger.LogInformation(FullLogMessage);

                Guid id = _logger.Log(ex);

                await context.Response.WriteAsJsonAsync(new
                {
                    message = $"An unexpected error has occured." +
                              $"Please contact support using this parameter: {id}"
                });

                throw;
            }
        }
    }
}
