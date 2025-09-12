using MediatR;
using StargateAPI.Business.Dtos;
using StargateAPI.Business.Services;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries;

public class GetPeople : IRequest<GetPeopleResult> { }

public class GetPeopleHandler(PersonAstronautService personAstronautService) 
    : IRequestHandler<GetPeople, GetPeopleResult>
{
    public readonly PersonAstronautService _personAstronautService = personAstronautService;

    public async Task<GetPeopleResult> Handle(GetPeople request, CancellationToken cancellationToken) =>
        new GetPeopleResult(
            people: await _personAstronautService.GetPersonAstronautsAsNoTrackingAsync(
                cancellationToken: cancellationToken));
}

public class GetPeopleResult(List<PersonAstronaut> people) : BaseResponse
{
    public readonly List<PersonAstronaut> People = people;
}
