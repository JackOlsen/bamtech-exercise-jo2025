using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Business.Services;
using System.Net;

namespace StargateAPI.Business.Queries;

public class GetAstronautDutiesByName(string name) : IRequest<GetAstronautDutiesByNameResult>
{
    public readonly string Name = name;
}

public class GetAstronautDutiesByNameHandler(StargateContext context, PersonAstronautService personAstronautService) 
    : IRequestHandler<GetAstronautDutiesByName, GetAstronautDutiesByNameResult>
{
    private readonly StargateContext _context = context;
    private readonly PersonAstronautService _personAstronautService = personAstronautService;

    public async Task<GetAstronautDutiesByNameResult> Handle(GetAstronautDutiesByName request, CancellationToken cancellationToken)
    {
        var person = await _personAstronautService.GetPersonAstronautAsNoTrackingAsync(
            name: request.Name,
            cancellationToken: cancellationToken)
            ?? throw new HttpRequestException(
                message: $"No astronaut found with name '{request.Name}'.",
                inner: null,
                statusCode: HttpStatusCode.NotFound);

        var duties = await _context.AstronautDuties
            .Where(d => d.PersonId == person.PersonId)
            .OrderByDescending(d => d.DutyStartDate)
            .ToListAsync(cancellationToken: cancellationToken);

        return new GetAstronautDutiesByNameResult
        {
            Person = person,
            AstronautDuties = duties
        };
    }
}

public class GetAstronautDutiesByNameResult
{
    public PersonAstronaut Person { get; init; } = null!;
    public List<AstronautDuty> AstronautDuties { get; init; } = null!;
}
