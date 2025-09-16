using MediatR;
using Microsoft.AspNetCore.Mvc;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;
using System.Net;

namespace StargateAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AstronautDutyController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("{name}")]
    [ProducesResponseType<GetAstronautDutiesByNameResult>(statusCode: (int)HttpStatusCode.OK)]
    [ProducesResponseType<ProblemDetails>(statusCode: (int)HttpStatusCode.NotFound)]
    public Task<GetAstronautDutiesByNameResult> GetAstronautDutiesByName(string name) =>
        _mediator.Send(new GetAstronautDutiesByName(name: name));

    [HttpPost]
    [ProducesResponseType<CreateAstronautDutyResult>(statusCode: (int)HttpStatusCode.Created)]
    [ProducesResponseType<ProblemDetails>(statusCode: (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType<ProblemDetails>(statusCode: (int)HttpStatusCode.NotFound)]
    [ProducesResponseType<ProblemDetails>(statusCode: (int)HttpStatusCode.Conflict)]
    public async Task<IActionResult> CreateAstronautDuty([FromBody] CreateAstronautDuty request) 
    {
        var createAstronautDutyResult = await _mediator.Send(request);
        return Created(
            uri: $"/astronautduty/{request.Name}",
            value: createAstronautDutyResult);
    }
}