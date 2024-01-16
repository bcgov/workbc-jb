using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace WorkBC.Web.Helpers;

public class BadHttpRequestExceptionFilter : IExceptionFilter
{
    private readonly ILogger _logger;
    
    // Constructor
    public BadHttpRequestExceptionFilter(ILogger logger)
    {
        _logger = logger;
    }
    
    public void OnException(ExceptionContext context)
    {
        if (context.ExceptionHandled) return;
        var exception = context.Exception;
        if (exception is not BadHttpRequestException) return;

        var fullErrorName = context.Exception.GetType().FullName;
        var endpoint = context.HttpContext.Request.Path;
        var method = context.HttpContext.Request.Method;
        
        // unable to log the payload since it's not completely received, hence this exception
        _logger.Error(fullErrorName + ": endpoint: " + endpoint + " method: " + method );
        var result = new ObjectResult(new
        {
            context.Exception.Message,
            context.Exception.Source,
            ExceptionType = fullErrorName
        })
        {
            StatusCode = (int?)HttpStatusCode.InternalServerError
        };
        context.Result = result;
    }
}