using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;
using System.Net;

namespace StargateAPI.Business.Commands;

public class CreatePerson : IRequest<CreatePersonResult>
{
    public required string Name { get; set; } = string.Empty;
}

public class CreatePersonPreProcessor(StargateContext context) 
    : IRequestPreProcessor<CreatePerson>
{
    private readonly StargateContext _context = context;

    public async Task Process(CreatePerson request, CancellationToken cancellationToken)
    {
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
    }
}

public class CreatePersonHandler(StargateContext context) 
    : IRequestHandler<CreatePerson, CreatePersonResult>
{
    private readonly StargateContext _context = context;

    public async Task<CreatePersonResult> Handle(CreatePerson request, CancellationToken cancellationToken)
    {
        var newPerson = new Person
        {
            Name = request.Name
        };

        await _context.People.AddAsync(
            entity: newPerson,
            cancellationToken: cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken: cancellationToken);

        return new CreatePersonResult(
            id: newPerson.Id);
    }
}

public class CreatePersonResult(int id) : BaseResponse
{
    public readonly int Id = id;
}
