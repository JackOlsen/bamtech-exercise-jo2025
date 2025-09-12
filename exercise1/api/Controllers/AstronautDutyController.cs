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
        this.GetResponse(
            response: await _mediator.Send(new GetPersonByName(name: name)));

    [HttpPost]
    public async Task<IActionResult> CreateAstronautDuty([FromBody] CreateAstronautDuty request) =>
        this.GetResponse(
            response: await _mediator.Send(request));
}