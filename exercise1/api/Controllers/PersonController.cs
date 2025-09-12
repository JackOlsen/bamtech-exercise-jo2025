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
    public async Task<IActionResult> GetPeople() =>
        this.GetResponse(
            response: await _mediator.Send(new GetPeople()));

    [HttpGet("{name}")]
    public async Task<IActionResult> GetPersonByName(string name) =>
        this.GetResponse(
            response: await _mediator.Send(new GetPersonByName(name: name)));

    [HttpPost]
    public async Task<IActionResult> CreatePerson([FromBody] string name) =>
        this.GetResponse(
            response: await _mediator.Send(new CreatePerson(name: name)));
}