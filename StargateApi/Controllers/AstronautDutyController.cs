using MediatR;
using Microsoft.AspNetCore.Mvc;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;
using StargateAPI.Utilities;

namespace StargateAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AstronautDutyController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("{name}")]
    [Ok<GetAstronautDutiesByNameResult>, NotFound]
    public Task<GetAstronautDutiesByNameResult> GetAstronautDutiesByName(string name) =>
        _mediator.Send(new GetAstronautDutiesByName(name: name));

    [HttpPost]
    [Created<CreateAstronautDutyResult>, BadRequest, NotFound, Conflict]
    public async Task<IActionResult> CreateAstronautDuty([FromBody] CreateAstronautDuty request) 
    {
        var createAstronautDutyResult = await _mediator.Send(request);
        return Created(
            uri: $"/astronautduty/{request.Name}",
            value: createAstronautDutyResult);
    }
}