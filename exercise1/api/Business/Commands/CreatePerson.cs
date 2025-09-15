using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using System.Net;

namespace StargateAPI.Business.Commands;

public class CreatePerson : IRequest<CreatePersonResult>
{
    public string Name { get; init; } = null!;
}

public class UpdatePerson : IRequest<UpdatePersonResult>
{
    public string CurrentName { get; init; } = null!;
    public string NewName { get; init; } = null!;
}

public class WritePersonHandler(StargateContext context) 
    : IRequestHandler<CreatePerson, CreatePersonResult>,
    IRequestHandler<UpdatePerson, UpdatePersonResult>
{
    private readonly StargateContext _context = context;

    public async Task<CreatePersonResult> Handle(CreatePerson request, CancellationToken cancellationToken)
    {
        var person = await _context.DoWithinTransaction(
            action: async ctx =>
            {
                await AssertIsNotDuplicateName(
                    name: request.Name,
                    cancellationToken: cancellationToken);

                var newPerson = new Person(
                    name: request.Name);

                await _context.People.AddAsync(
                    entity: newPerson,
                    cancellationToken: cancellationToken);

                return newPerson;
            },
            cancellationToken: cancellationToken);

        return new CreatePersonResult
        {
            Id = person.Id
        };
    }

    public async Task<UpdatePersonResult> Handle(UpdatePerson request, CancellationToken cancellationToken)
    {
        var person = await _context.DoWithinTransaction(
            action: async ctx =>
            {
                await AssertIsNotDuplicateName(
                    name: request.NewName,
                    cancellationToken: cancellationToken);

                var existingPerson = await _context.People
                    .Where(p => p.Name == request.CurrentName)
                    .FirstOrDefaultAsync(cancellationToken: cancellationToken)
                    ?? throw new HttpRequestException(
                        message: $"No person found with name '{request.CurrentName}'.",
                        inner: null,
                        statusCode: HttpStatusCode.NotFound);

                existingPerson.Name = request.NewName;

                return existingPerson;
            },
            cancellationToken: cancellationToken);

        return new UpdatePersonResult
        {
            Id = person.Id
        };
    }

    private async Task AssertIsNotDuplicateName(string name, CancellationToken cancellationToken)
    {
        if (await _context.People.AsNoTracking()
            .AnyAsync(
                predicate: z => z.Name == name,
                cancellationToken: cancellationToken))
        {
            throw new HttpRequestException(
                message: $"Duplicate astronaut name '{name}'.",
                inner: null,
                statusCode: HttpStatusCode.Conflict);
        }
    }
}

public class CreatePersonResult
{
    public int Id { get; init; }
}

public class UpdatePersonResult
{
    public int Id { get; init; }
}
