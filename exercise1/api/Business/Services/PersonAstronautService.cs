using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using System.Linq.Expressions;

namespace StargateAPI.Business.Services;

public class PersonAstronautService(StargateContext context)
{
    private readonly StargateContext _context = context;

    public Task<PersonAstronaut?> GetPersonAstronautAsync(string name, CancellationToken cancellationToken) =>
        _context.People
            .Select(GetPersonAstronaut)
            .FirstOrDefaultAsync(
                predicate: p => p.Name == name,
                cancellationToken: cancellationToken);

    public Task<List<PersonAstronaut>> GetPersonAstronautsAsync(CancellationToken cancellationToken) =>
        _context.People
            .Select(GetPersonAstronaut)
            .ToListAsync(cancellationToken: cancellationToken);

    private static readonly Expression<Func<Person, PersonAstronaut>> GetPersonAstronaut = p =>
        p.AstronautDetail == null
            ? new PersonAstronaut
            {
                PersonId = p.Id,
                Name = p.Name
            }
            : new PersonAstronaut
            {
                PersonId = p.Id,
                Name = p.Name,
                CurrentRank = p.AstronautDetail.CurrentRank,
                CurrentDutyTitle = p.AstronautDetail.CurrentDutyTitle,
                CareerStartDate = p.AstronautDetail.CareerStartDate,
                CareerEndDate = p.AstronautDetail.CareerEndDate,
            };
}