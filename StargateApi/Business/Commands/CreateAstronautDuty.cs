using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Logging;
using System.Net;

namespace StargateAPI.Business.Commands;

public class CreateAstronautDuty : IRequest<CreateAstronautDutyResult>
{
    public required string Name { get; init; }
    public required string Rank { get; init; }
    public required string DutyTitle { get; init; }
    public DateTime DutyStartDate { get; init; }
}

public class CreateAstronautDutyHandler(
    StargateContext context,
    ProcessLoggingService processLoggingService) 
    : IRequestHandler<CreateAstronautDuty, CreateAstronautDutyResult>
{
    private readonly StargateContext _context = context;
    private readonly ProcessLoggingService _processLoggingService = processLoggingService;

    public async Task<CreateAstronautDutyResult> Handle(
        CreateAstronautDuty request,
        CancellationToken cancellationToken)
    {
        _processLoggingService.InitiateLogEntry(
            description: "Create Astronaut Duty",
            details:
            [
                ("Name", request.Name),
                ("Rank", request.Rank),
                ("DutyTitle", request.DutyTitle),
                ("DutyStartDate", request.DutyStartDate.ToString("yyyy-MM-dd"))
            ]);
        var person = await _context.People
            .FirstOrDefaultAsync(
                predicate: p => p.Name == request.Name,
                cancellationToken: cancellationToken)
            ?? throw new HttpRequestException(
                message: $"No astronaut found with name '{request.Name}'.",
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
                message: "Duplicate astronaut duty.",
                inner: null,
                statusCode: HttpStatusCode.Conflict);
        }

        var astronautDetail = await _context.AstronautDetails
            .FirstOrDefaultAsync(
                predicate: d => d.PersonId == person.Id,
                cancellationToken: cancellationToken);

        var oneDayBeforeNewDutyStartDate = request.DutyStartDate.AddDays(-1);
        if (astronautDetail == null)
        {
            await _context.AstronautDetails.AddAsync(
                entity: AstronautDetail.ForPerson(
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
                astronautDetail.CareerEndDate = oneDayBeforeNewDutyStartDate;
            }
            _context.AstronautDetails.Update(astronautDetail);
        }

        var previousAstronautDuty = await _context.AstronautDuties
            .Where(d => d.PersonId == person.Id)
            .OrderByDescending(d => d.DutyStartDate)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (previousAstronautDuty != null)
        {
            previousAstronautDuty.DutyEndDate = oneDayBeforeNewDutyStartDate;
            _context.AstronautDuties.Update(previousAstronautDuty);
        }

        var newAstronautDuty = AstronautDuty.ForPerson(
            personId: person.Id,
            rank: request.Rank,
            dutyTitle: request.DutyTitle,
            dutyStartDate: request.DutyStartDate.Date,
            dutyEndDate: null);

        await _context.AstronautDuties.AddAsync(newAstronautDuty, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken: cancellationToken);

        return new CreateAstronautDutyResult
        {
            Id = newAstronautDuty.Id
        };
    }
}

public class CreateAstronautDutyResult
{
    public int? Id { get; init; }
}
