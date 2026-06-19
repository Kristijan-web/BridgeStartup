using Application.Exceptions;
using FluentValidation;

namespace ASPLAB2.API.Middleware
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        //private readonly IExceptionLogger _logger;
        public GlobalExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
            //_logger = logger;
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

                    var errors = new
                    {
                        message = loginException.Message
                    };

                    await context.Response.WriteAsJsonAsync(errors);
                    return;

                }

                if (ex is EntityNotFoundException EntityException)
                {
                    context.Response.StatusCode = 401;

                    var message = new
                    {
                        message = EntityException.Message
                    };

                    await context.Response.WriteAsJsonAsync(message);

                    return;

                }


                //if (ex is UnauthorizedUseCaseException)
                //{
                //    context.Response.StatusCode = 401;
                //    return;
                //}

                context.Response.StatusCode = 500;
                //Guid id = _logger.Log(ex);
                //Generalni izuzetak (neocekivan) -> Response code 500, logging

                await context.Response.WriteAsJsonAsync(new
                {
                    message = "An unexpected error has occured. " +
                              $"Please contact support using this parameter: "
                });

                throw;
            }
        }
    }
}
