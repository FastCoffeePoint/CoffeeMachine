using Cmb.Domain;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;

namespace Cmb.Application;

public class OrderExecutionProcess(IOptionsMonitor<CoffeeMachineConfiguration> configuration)
{
    public async Task<Result<bool>> Execute(CoffeeWasOrderedEvent form)
    {
        // get info about ingredient amounts, validate
        
        
        
        return Result.Success(true);
    }
}