using MediatR;
using Microsoft.AspNetCore.Mvc;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;

namespace StargateAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AstronautDutyController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("{name}")]
    public async Task<IActionResult> GetAstronautDutiesByName(string name) =>
        Ok(await _mediator.Send(new GetAstronautDutiesByName(name: name)));

    [HttpPost]
    public async Task<IActionResult> CreateAstronautDuty([FromBody] CreateAstronautDuty request) 
    {
        var createAstronautDutyResult = await _mediator.Send(request);
        return Created(
            uri: $"/astronautduty/{request.Name}",
            value: createAstronautDutyResult);
    }
}