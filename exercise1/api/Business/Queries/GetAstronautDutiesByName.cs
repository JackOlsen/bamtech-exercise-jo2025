using Dapper;
using MediatR;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Controllers;
using System.Net;

namespace StargateAPI.Business.Queries;

public class GetAstronautDutiesByName : IRequest<GetAstronautDutiesByNameResult>
{
    public string Name { get; set; } = string.Empty;
}

public class GetAstronautDutiesByNameHandler(StargateContext context) 
    : IRequestHandler<GetAstronautDutiesByName, GetAstronautDutiesByNameResult>
{
    private readonly StargateContext _context = context;

    public async Task<GetAstronautDutiesByNameResult> Handle(GetAstronautDutiesByName request, CancellationToken cancellationToken)
    {
        // TODO: Use DbContext
        var person = await _context.Connection.QueryFirstOrDefaultAsync<PersonAstronaut>(
            sql: $"SELECT a.Id as PersonId, a.Name, b.CurrentRank, b.CurrentDutyTitle, b.CareerStartDate, b.CareerEndDate FROM [Person] a LEFT JOIN [AstronautDetail] b on b.PersonId = a.Id WHERE \'{request.Name}\' = a.Name")
            ?? throw new HttpRequestException(
                message: $"No person found with name '{request.Name}'.", 
                inner: null,
                statusCode: HttpStatusCode.NotFound);

        // TODO: Use DbContext
        var duties = await _context.Connection.QueryAsync<AstronautDuty>(
            sql: $"SELECT * FROM [AstronautDuty] WHERE {person.PersonId} = PersonId ORDER BY DutyStartDate DESC");

        return new GetAstronautDutiesByNameResult(
            person: person,
            astronautDuties: [..duties]);
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
