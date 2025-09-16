namespace StargateAPI.Business.Data;

public static class StargateContextExtensions
{
    public static async Task<TResult> DoWithinTransaction<TResult>(
        this StargateContext context,
        Func<StargateContext, Task<TResult>> action,
        CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(
            cancellationToken: cancellationToken);

        var result = await action(context);

        await context.SaveChangesAsync(
            cancellationToken: cancellationToken);

        await transaction.CommitAsync(cancellationToken: cancellationToken);

        return result;
    }
}