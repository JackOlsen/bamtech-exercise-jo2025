using StargateAPI.Business.Data;
using System.Diagnostics;

namespace StargateAPI.Business.Logging;

public class ProcessLoggingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(
        HttpContext httpContext,
        ProcessLoggingService processLoggingService,
        StargateContext dbContext)
    {
        var timestamp = Stopwatch.GetTimestamp();
        var stopwatch = Stopwatch.StartNew();

        await _next(httpContext);

        stopwatch.Stop();

        if (!processLoggingService.TryGetCurrentLogInfo(out var logInfo))
        {
            return;
        }

        dbContext.LogEntries.Add(new LogEntry(
            timestamp: new DateTimeOffset(timestamp, TimeSpan.Zero),
            description: logInfo.Description!,
            detail: string.Join(", ", (logInfo.Details ?? []).Select(e => $"{e.Key}: '{e.Value}'")),
            success: logInfo.Success,
            error: logInfo.Error,
            elapsed: stopwatch.ElapsedMilliseconds));
        await dbContext.SaveChangesAsync();
    }
}

public class ProcessLoggingService
{
    private bool IsLoggingInitiated;
    private string? Description;
    private (string Key, string Value)[]? Details;
    private bool Success = true;
    private string? Error;

    public void InitiateLogEntry(
        string description, 
        params (string Key, string Value)[] details)
    {
        if(IsLoggingInitiated)
        {
            throw new InvalidOperationException(
                "Logging has already been initiated for this request.");
        }
        Description = description;
        Details = details ?? [];
        IsLoggingInitiated = true;
    }

    public void RecordError(string error)
    {
        Success = false;
        Error = error;
    }

    public bool TryGetCurrentLogInfo(
        out (string? Description, (string Key, string Value)[]? Details, bool Success, string? Error) logEntry)
    {
        if(!IsLoggingInitiated)
        {
            logEntry = (null, null, false, null);
            return false;
        }
        logEntry = (Description, Details, Success, Error);
        return true;
    }
}