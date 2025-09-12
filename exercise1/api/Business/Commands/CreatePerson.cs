using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;
using System.Net;

namespace StargateAPI.Business.Commands;

public class CreatePerson : IRequest<CreatePersonResult>
{
    public string Name { get; set; } = null!;
}

public class UpdatePerson : IRequest<UpdatePersonResult>
{
    public string CurrentName { get; set; } = null!;
    public string NewName { get; set; } = null!;
}

public class WritePersonHandler(StargateContext context) 
    : IRequestHandler<CreatePerson, CreatePersonResult>,
    IRequestHandler<UpdatePerson, UpdatePersonResult>
{
    private readonly StargateContext _context = context;

    public async Task<CreatePersonResult> Handle(CreatePerson request, CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(
            cancellationToken: cancellationToken);

        await AssertIsNotDuplicateName(
            name: request.Name,
            cancellationToken: cancellationToken);

        var newPerson = new Person(
            name: request.Name);

        await _context.People.AddAsync(
            entity: newPerson,
            cancellationToken: cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken: cancellationToken);

        await transaction.CommitAsync(cancellationToken: cancellationToken);

        return new CreatePersonResult(
            id: newPerson.Id);
    }

    public async Task<UpdatePersonResult> Handle(UpdatePerson request, CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(
            cancellationToken: cancellationToken);

        await AssertIsNotDuplicateName(
            name: request.NewName,
            cancellationToken: cancellationToken);

        var person = await _context.People
            .Where(p => p.Name == request.CurrentName)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken)
            ?? throw new HttpRequestException(
                message: $"No person found with name '{request.CurrentName}'.",
                inner: null,
                statusCode: HttpStatusCode.NotFound);

        person.Name = request.NewName;

        await _context.SaveChangesAsync(
            cancellationToken: cancellationToken);

        await transaction.CommitAsync(cancellationToken: cancellationToken);

        return new UpdatePersonResult(
            id: person.Id);
    }

    private async Task AssertIsNotDuplicateName(string name, CancellationToken cancellationToken)
    {
        if (await _context.People.AsNoTracking()
            .AnyAsync(
                predicate: z => z.Name == name,
                cancellationToken: cancellationToken))
        {
            throw new HttpRequestException(
                message: $"Duplicate astronaut name '{name}'",
                inner: null,
                statusCode: HttpStatusCode.Conflict);
        }
    }
}

public class CreatePersonResult(int id) : BaseResponse
{
    public readonly int Id = id;
}

public class UpdatePersonResult(int id) : BaseResponse
{
    public readonly int Id = id;
}
