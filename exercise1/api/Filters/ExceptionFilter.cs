using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Net;

namespace StargateAPI.Filters;

public class ExceptionFilter(
    ProblemDetailsFactory problemDetailsFactory,
    ILogger<ExceptionFilter> logger) 
    : IExceptionFilter
{
    private readonly ProblemDetailsFactory _problemDetailsFactory = problemDetailsFactory;
    private readonly ILogger<ExceptionFilter> _logger = logger;

    public void OnException(ExceptionContext context)
    {
        var (httpStatusCode, title) = context.Exception switch
        {
            BadHttpRequestException badHttpEx => (StatusCodes.Status400BadRequest, badHttpEx.Message),
            HttpRequestException httpEx => ((int?)httpEx.StatusCode, httpEx.Message),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred"),
        };

        if (httpStatusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(context.Exception, "An unhandled exception occurred.");
        }

        context.Result = new ObjectResult(
            value: _problemDetailsFactory.CreateProblemDetails(
                context.HttpContext,
                statusCode: httpStatusCode,
                title: title,
                detail: context.Exception.Message)) 
        { 
            StatusCode = httpStatusCode
        };
        context.ExceptionHandled = true; 
    }
}