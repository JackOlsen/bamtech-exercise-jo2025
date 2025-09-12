using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Business.Services;
using StargateAPI.Controllers;
using System.Net;

namespace StargateAPI.Business.Queries;

public class GetAstronautDutiesByName : IRequest<GetAstronautDutiesByNameResult>
{
    public string Name { get; set; } = string.Empty;
}

public class GetAstronautDutiesByNameHandler(StargateContext context, PersonAstronautService personAstronautService) 
    : IRequestHandler<GetAstronautDutiesByName, GetAstronautDutiesByNameResult>
{
    private readonly StargateContext _context = context;
    private readonly PersonAstronautService _personAstronautService = personAstronautService;

    public async Task<GetAstronautDutiesByNameResult> Handle(GetAstronautDutiesByName request, CancellationToken cancellationToken)
    {
        var person = await _personAstronautService.GetPersonAstronautAsync(
            name: request.Name,
            cancellationToken: cancellationToken)
            ?? throw new HttpRequestException(
                message: $"No person found with name '{request.Name}'.", 
                inner: null,
                statusCode: HttpStatusCode.NotFound);

        var duties = await _context.AstronautDuties
            .Where(d => d.PersonId == person.PersonId)
            .OrderByDescending(d => d.DutyStartDate)
            .ToListAsync(cancellationToken: cancellationToken);

        return new GetAstronautDutiesByNameResult(
            person: person,
            astronautDuties: duties);
    }
}

public class GetAstronautDutiesByNameResult(
    PersonAstronaut person, 
    List<AstronautDuty> astronautDuties) 
    : BaseResponse
{
    public readonly PersonAstronaut Person = person;
    public readonly List<AstronautDuty> AstronautDuties = astronautDuties;
}
