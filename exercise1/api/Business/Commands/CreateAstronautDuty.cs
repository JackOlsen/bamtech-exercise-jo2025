using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;
using System.Net;

namespace StargateAPI.Business.Commands;

public class CreateAstronautDuty : IRequest<CreateAstronautDutyResult>
{
    public required string Name { get; set; }

    public required string Rank { get; set; }

    public required string DutyTitle { get; set; }

    public DateTime DutyStartDate { get; set; }
}

public class CreateAstronautDutyHandler(StargateContext context) 
    : IRequestHandler<CreateAstronautDuty, CreateAstronautDutyResult>
{
    private readonly StargateContext _context = context;

    public async Task<CreateAstronautDutyResult> Handle(
        CreateAstronautDuty request,
        CancellationToken cancellationToken)
    {
        var person = await _context.People
            .FirstOrDefaultAsync(
                predicate: p => p.Name == request.Name,
                cancellationToken: cancellationToken)
            ?? throw new HttpRequestException(
                message: $"No person found with name '{request.Name}'.",
                inner: null,
                statusCode: HttpStatusCode.NotFound);

        await using var transaction = await _context.Database.BeginTransactionAsync(
            cancellationToken: cancellationToken);

        if (await _context.AstronautDuties
            .AnyAsync(
                predicate: z => z.Person.Name == request.Name
                    && z.DutyTitle == request.DutyTitle
                    && z.DutyStartDate == request.DutyStartDate,
                cancellationToken: cancellationToken))
        {
            throw new HttpRequestException(
                message: "Duplicate astronaut duty",
                inner: null,
                statusCode: HttpStatusCode.Conflict);
        }

        var astronautDetail = await _context.AstronautDetails
            .FirstOrDefaultAsync(
                predicate: d => d.PersonId == person.Id,
                cancellationToken: cancellationToken);

        if (astronautDetail == null)
        {
            await _context.AstronautDetails.AddAsync(
                entity: new AstronautDetail(
                    personId: person.Id,
                    currentDutyTitle: request.DutyTitle,
                    currentRank: request.Rank,
                    careerStartDate: request.DutyStartDate.Date,
                    careerEndDate: request.DutyTitle == Constants.DutyTitles.RETIRED
                        ? request.DutyStartDate.Date
                        : null),
                cancellationToken: cancellationToken);
        }
        else
        {
            astronautDetail.CurrentDutyTitle = request.DutyTitle;
            astronautDetail.CurrentRank = request.Rank;
            if (request.DutyTitle == Constants.DutyTitles.RETIRED)
            {
                astronautDetail.CareerEndDate = request.DutyStartDate.AddDays(-1).Date;
            }
            _context.AstronautDetails.Update(astronautDetail);
        }

        var astronautDuty = await _context.AstronautDuties
            .Where(d => d.PersonId == person.Id)
            .OrderByDescending(d => d.DutyStartDate)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (astronautDuty != null)
        {
            astronautDuty.DutyEndDate = request.DutyStartDate.AddDays(-1).Date;
            _context.AstronautDuties.Update(astronautDuty);
        }

        var newAstronautDuty = new AstronautDuty(
            personId: person.Id,
            rank: request.Rank,
            dutyTitle: request.DutyTitle,
            dutyStartDate: request.DutyStartDate.Date,
            dutyEndDate: null);

        await _context.AstronautDuties.AddAsync(newAstronautDuty, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken: cancellationToken);

        return new CreateAstronautDutyResult(
            id: newAstronautDuty.Id);
    }
}

public class CreateAstronautDutyResult(int? id) : BaseResponse
{
    public readonly int? Id = id;
}
