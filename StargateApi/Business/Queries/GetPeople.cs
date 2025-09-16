using MediatR;
using StargateAPI.Business.Dtos;
using StargateAPI.Business.Services;

namespace StargateAPI.Business.Queries;

public class GetPeople : IRequest<GetPeopleResult> { }

public class GetPeopleHandler(PersonAstronautService personAstronautService) 
    : IRequestHandler<GetPeople, GetPeopleResult>
{
    public readonly PersonAstronautService _personAstronautService = personAstronautService;

    public async Task<GetPeopleResult> Handle(GetPeople request, CancellationToken cancellationToken) =>
        new GetPeopleResult
        {
            People = await _personAstronautService.GetPersonAstronautsAsNoTrackingAsync(
                cancellationToken: cancellationToken)
        };
}

public class GetPeopleResult
{
    public List<PersonAstronaut> People { get; init; } = null!;
}
