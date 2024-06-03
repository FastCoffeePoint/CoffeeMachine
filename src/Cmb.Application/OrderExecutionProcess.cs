using Cmb.Domain;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;

namespace Cmb.Application;

public class OrderExecutionProcess(IOptionsMonitor<CoffeeMachineConfiguration> configuration)
{
    public async Task<Result<bool>> Execute(CoffeeWasOrderedEvent form)
    {
        return Result.Success(true);
    }
}