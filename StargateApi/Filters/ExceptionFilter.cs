using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using StargateAPI.Business.Logging;
using System.Net;

namespace StargateAPI.Filters;

public class ExceptionFilter(
    ProblemDetailsFactory problemDetailsFactory,
    ILogger<ExceptionFilter> logger,
    ProcessLoggingService processLoggingService) 
    : IExceptionFilter
{
    private readonly ProblemDetailsFactory _problemDetailsFactory = problemDetailsFactory;
    private readonly ILogger<ExceptionFilter> _logger = logger;
    private readonly ProcessLoggingService _processLoggingService = processLoggingService;

    public void OnException(ExceptionContext context)
    {
        var httpStatusCode = context.Exception switch
        {
            BadHttpRequestException => HttpStatusCode.BadRequest,
            HttpRequestException httpEx => (HttpStatusCode)httpEx.StatusCode!,
            _ => HttpStatusCode.InternalServerError,
        };

        if (httpStatusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(context.Exception, "An unhandled exception occurred.");
        }

        context.Result = new ObjectResult(
            value: _problemDetailsFactory.CreateProblemDetails(
                httpContext: context.HttpContext,
                statusCode: (int)httpStatusCode,
                title: STATUS_CODE_TITLES[httpStatusCode!],
                detail: context.Exception.Message))
        {
            StatusCode = (int?)httpStatusCode
        };
        _processLoggingService.RecordError(error: context.Exception.Message);
        context.ExceptionHandled = true; 
    }

    /// <summary>
    /// Human-readable titles for HTTP status codes, i.e.: "Not Found" instead of "NotFound".
    /// </summary>
    private static readonly Dictionary<HttpStatusCode, string> STATUS_CODE_TITLES = 
        Enum.GetValues<HttpStatusCode>()
            .Distinct()
            .ToDictionary(
                keySelector: s => s,    
                elementSelector: s => string.Concat(s.ToString()!
                    .Select(x => char.IsUpper(x) ? " " + x : x.ToString()))
                    .TrimStart(' '));
}