using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WorkBC.Web.Helpers;

public class BadHttpRequestExceptionFilter : IExceptionFilter
{
    
    public void OnException(ExceptionContext context)
    {
        if (context.ExceptionHandled) return;
        var exception = context.Exception;
        if (exception is not BadHttpRequestException) return;
        
        var result = new ObjectResult(new
        {
            context.Exception.Message,
            context.Exception.Source,
            ExceptionType = context.Exception.GetType().FullName
        })
        {
            StatusCode = (int?)HttpStatusCode.InternalServerError
        };
        context.Result = result;
    }
}