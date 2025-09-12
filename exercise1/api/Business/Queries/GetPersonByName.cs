using MediatR;
using StargateAPI.Business.Dtos;
using StargateAPI.Business.Services;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries;

public class GetPersonByName(string name) : IRequest<GetPersonByNameResult>
{
    public readonly string Name = name;
}

public class GetPersonByNameHandler(PersonAstronautService personAstronautService)
    : IRequestHandler<GetPersonByName, GetPersonByNameResult>
{
    private readonly PersonAstronautService _personAstronautService = personAstronautService;

    public async Task<GetPersonByNameResult> Handle(GetPersonByName request, CancellationToken cancellationToken)
    {
        var person = await _personAstronautService.GetPersonAstronautAsNoTrackingAsync(
            name: request.Name,
            cancellationToken: cancellationToken);

        return new GetPersonByNameResult
        {
            Person = person
        };
    }
}

public class GetPersonByNameResult : BaseResponse
{
    public PersonAstronaut? Person { get; set; }
}
