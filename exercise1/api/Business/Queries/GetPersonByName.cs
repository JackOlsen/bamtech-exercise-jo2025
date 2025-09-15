using MediatR;
using StargateAPI.Business.Dtos;
using StargateAPI.Business.Services;
using System.Net;

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
            cancellationToken: cancellationToken)
            ?? throw new HttpRequestException(
                message: $"No person found with name '{request.Name}'.",
                inner: null,
                statusCode: HttpStatusCode.NotFound);

        return new GetPersonByNameResult
        {
            Person = person
        };
    }
}

public class GetPersonByNameResult
{
    public PersonAstronaut? Person { get; init; }
}
