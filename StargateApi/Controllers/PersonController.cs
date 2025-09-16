using MediatR;
using Microsoft.AspNetCore.Mvc;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Dtos;
using StargateAPI.Business.Queries;
using System.Net;

namespace StargateAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PersonController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    [ProducesResponseType<GetPeopleResult>(statusCode: (int)HttpStatusCode.OK)]
    public Task<GetPeopleResult> GetPeople() =>
        _mediator.Send(new GetPeople());

    [HttpGet("{name}")]
    [ProducesResponseType<GetPersonByNameResult>(statusCode: (int)HttpStatusCode.OK)]
    [ProducesResponseType<ProblemDetails>(statusCode: (int)HttpStatusCode.NotFound)]
    public Task<GetPersonByNameResult> GetPersonByName(string name) =>
        _mediator.Send(new GetPersonByName(name: name));

    [HttpPost]
    [ProducesResponseType<CreatePersonResult>(statusCode: (int)HttpStatusCode.Created)]
    [ProducesResponseType<ProblemDetails>(statusCode: (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType<ProblemDetails>(statusCode: (int)HttpStatusCode.Conflict)]
    public async Task<CreatedResult> CreatePerson([FromBody] CreatePerson request) 
    {
        var createPersonResult = await _mediator.Send(request);
        return Created(
            uri: $"/person/{request.Name}",
            value: createPersonResult);
    }

    [HttpPut("{name}")]
    [ProducesResponseType<UpdatePersonResult>(statusCode: (int)HttpStatusCode.OK)]
    [ProducesResponseType<ProblemDetails>(statusCode: (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType<ProblemDetails>(statusCode: (int)HttpStatusCode.NotFound)]
    [ProducesResponseType<ProblemDetails>(statusCode: (int)HttpStatusCode.Conflict)]
    public Task<UpdatePersonResult> UpdatePerson(string name, [FromBody] UpdatePersonInput input) =>
        _mediator.Send(new UpdatePerson
        {
            CurrentName = name,
            NewName = input.NewName
        });
}