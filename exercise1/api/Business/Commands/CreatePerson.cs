using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;
using System.Net;

namespace StargateAPI.Business.Commands;

public class CreatePerson(string name) : IRequest<CreatePersonResult>
{
    public readonly string Name = name;
}

public class CreatePersonHandler(StargateContext context) 
    : IRequestHandler<CreatePerson, CreatePersonResult>
{
    private readonly StargateContext _context = context;

    public async Task<CreatePersonResult> Handle(CreatePerson request, CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(
            cancellationToken: cancellationToken);

        if (await _context.People.AsNoTracking()
            .AnyAsync(
                predicate: z => z.Name == request.Name,
                cancellationToken: cancellationToken))
        {
            throw new HttpRequestException(
                message: "Duplicate astronaut name",
                inner: null,
                statusCode: HttpStatusCode.Conflict);
        }

        var newPerson = new Person
        {
            Name = request.Name
        };

        await _context.People.AddAsync(
            entity: newPerson,
            cancellationToken: cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken: cancellationToken);

        await transaction.CommitAsync(cancellationToken: cancellationToken);

        return new CreatePersonResult(
            id: newPerson.Id);
    }
}

public class CreatePersonResult(int id) : BaseResponse
{
    public readonly int Id = id;
}
