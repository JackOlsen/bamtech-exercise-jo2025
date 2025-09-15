using MediatR;
using Microsoft.AspNetCore.Mvc;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;

namespace StargateAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PersonController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    // TODO: Add ProducesResponseTypeAttributes to all actions
    public async Task<IActionResult> GetPeople() =>
        Ok(await _mediator.Send(new GetPeople()));

    [HttpGet("{name}")]
    public async Task<IActionResult> GetPersonByName(string name) =>
        Ok(await _mediator.Send(new GetPersonByName(name: name)));

    [HttpPost]
    public async Task<IActionResult> CreatePerson([FromBody] CreatePerson request) =>
        Ok(await _mediator.Send(request));

    [HttpPut]
    public async Task<IActionResult> UpdatePerson([FromBody] UpdatePerson request) =>
        Ok(await _mediator.Send(request));
}