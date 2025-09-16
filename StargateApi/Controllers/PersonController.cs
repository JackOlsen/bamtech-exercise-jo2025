using MediatR;
using Microsoft.AspNetCore.Mvc;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Dtos;
using StargateAPI.Business.Queries;
using StargateAPI.Utilities;

namespace StargateAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PersonController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    [Ok<GetPeopleResult>]
    public Task<GetPeopleResult> GetPeople() =>
        _mediator.Send(new GetPeople());

    [HttpGet("{name}")]
    [Ok<GetPersonByNameResult>, NotFound]
    public Task<GetPersonByNameResult> GetPersonByName(string name) =>
        _mediator.Send(new GetPersonByName(name: name));

    [HttpPost]
    [Created<CreatePersonResult>, BadRequest, Conflict]
    public async Task<CreatedResult> CreatePerson([FromBody] CreatePerson request) 
    {
        var createPersonResult = await _mediator.Send(request);
        return Created(
            uri: $"/person/{request.Name}",
            value: createPersonResult);
    }

    [HttpPut("{name}")]
    [Ok<UpdatePersonResult>, BadRequest, NotFound, Conflict]
    public Task<UpdatePersonResult> UpdatePerson(string name, [FromBody] UpdatePersonInput input) =>
        _mediator.Send(new UpdatePerson
        {
            CurrentName = name,
            NewName = input.NewName
        });
}