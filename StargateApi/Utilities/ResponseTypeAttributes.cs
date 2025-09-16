using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace StargateAPI.Utilities;

public class OkAttribute<TResponse>()
    : ProducesResponseTypeAttribute<TResponse>(
        statusCode: (int)HttpStatusCode.OK)
{}

public class CreatedAttribute<TResponse>()
    : ProducesResponseTypeAttribute<TResponse>(
        statusCode: (int) HttpStatusCode.Created)
{}

public class NotFound()
    : ProducesResponseTypeAttribute<ProblemDetails>(
        statusCode: (int)HttpStatusCode.NotFound)
{ }

public class BadRequest()
    : ProducesResponseTypeAttribute<ProblemDetails>(
        statusCode: (int)HttpStatusCode.BadRequest)
{ }

public class Conflict()
    : ProducesResponseTypeAttribute<ProblemDetails>(
        statusCode: (int)HttpStatusCode.Conflict)
{ }