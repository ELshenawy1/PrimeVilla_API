using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PrimeVilla_Web.Services;

namespace PrimeVilla_Web.Extensions
{
    public class AuthExceptionRedirection : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is AuthException)
                context.Result = new RedirectToActionResult("Login", "Auth", null);
        }
    }
}
