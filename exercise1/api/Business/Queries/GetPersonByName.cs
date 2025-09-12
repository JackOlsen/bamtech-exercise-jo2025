using Dapper;
using MediatR;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries;

public class GetPersonByName(string name) : IRequest<GetPersonByNameResult>
{
    public readonly string Name = name;
}

public class GetPersonByNameHandler(StargateContext context) 
    : IRequestHandler<GetPersonByName, GetPersonByNameResult>
{
    private readonly StargateContext _context = context;

    public async Task<GetPersonByNameResult> Handle(GetPersonByName request, CancellationToken cancellationToken)
    {            
        // TODO: Use DbContext
        var person = await _context.Connection.QueryFirstOrDefaultAsync<PersonAstronaut>(
            sql: $"SELECT a.Id as PersonId, a.Name, b.CurrentRank, b.CurrentDutyTitle, b.CareerStartDate, b.CareerEndDate FROM [Person] a LEFT JOIN [AstronautDetail] b on b.PersonId = a.Id WHERE '{request.Name}' = a.Name");

        return new GetPersonByNameResult(
            person: person);
    }
}

public class GetPersonByNameResult(PersonAstronaut? person) : BaseResponse
{
    public readonly PersonAstronaut? Person = person;
}
