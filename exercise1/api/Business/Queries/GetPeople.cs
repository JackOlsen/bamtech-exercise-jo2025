using Dapper;
using MediatR;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries;

public class GetPeople : IRequest<GetPeopleResult> { }

public class GetPeopleHandler(StargateContext context) 
    : IRequestHandler<GetPeople, GetPeopleResult>
{
    public readonly StargateContext _context = context;

    public async Task<GetPeopleResult> Handle(GetPeople request, CancellationToken cancellationToken)
    {
        // TODO: Use DbContext
        var people = await _context.Connection.QueryAsync<PersonAstronaut>(
            sql: $"SELECT a.Id as PersonId, a.Name, b.CurrentRank, b.CurrentDutyTitle, b.CareerStartDate, b.CareerEndDate FROM [Person] a LEFT JOIN [AstronautDetail] b on b.PersonId = a.Id");

        return new GetPeopleResult(
            people: [..people]);
    }
}

public class GetPeopleResult(List<PersonAstronaut> people) : BaseResponse
{
    public readonly List<PersonAstronaut> People = people;
}
