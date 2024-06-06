using Cmb.Application.Sensors;
using Cmb.Common.Kafka;
using Cmb.Domain;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using Serilog;

namespace Cmb.Application;

public class OrderExecutionProcess(IOptionsMonitor<CoffeeMachineConfiguration> _configuration, 
    ICoffeePresenceChecker _coffeePresenceChecker,    
    IIngredientsSensor _ingredientsSensor,
    IRecipesSensor _recipesSensor,
    IKafkaProducer _kafkaProducer)
{
    public async Task<Result<bool>> Execute(CoffeeWasOrderedEvent form)
    {
        //TODO: check a coffee presence
        
        var ingredients = _configuration.CurrentValue.Ingredients;
        var amountTasksBeforeExecution = ingredients.Select(async u => (u.IngredientId, Amount: await _ingredientsSensor.GetAmount(u.SensorId)));
        var amountResultsBeforeExecution = await Task.WhenAll(amountTasksBeforeExecution);
        if (amountResultsBeforeExecution.Any(u => u.Amount.IsFailure))
            return await AmountCountingError(amountResultsBeforeExecution, form, false);
                
        //TODO: Deciding to commit an event or not, but where can I get ingredients?   
        
        var recipeSensor = _configuration.CurrentValue.Recipes.FirstOrDefault(u => u.RecipeId == form.RecipeId);
        if (recipeSensor == null)
            return await RecipeIsNotFoundError(form);
            
        await _kafkaProducer.Push(new CoffeeStartedBrewingEvent(form.OrderId, _configuration.CurrentValue.MachineId));
        await _recipesSensor.StartCooking(recipeSensor.SensorId);
        
        var amountTasksAfterExecution = ingredients.Select(async u => (u.IngredientId, Amount: await _ingredientsSensor.GetAmount(u.SensorId)));
        var amountResultsAfterExecution = await Task.WhenAll(amountTasksAfterExecution);
        if (amountResultsAfterExecution.Any(u => u.Amount.IsFailure))
            return await AmountCountingError(amountResultsAfterExecution, form, true);

        var isCoffeeTaken = await WaitUntilCoffeeWillBeTaken(5); //TODO: move 5 to config
        //TODO: probably alert? Or infinite waiting?
        
        return Result.Success(true);
    }

    private async Task<bool> WaitUntilCoffeeWillBeTaken(int waitingMinutes)
    {
        var delay = new TimeSpan(0, 0, 0, 1); 
        var startTime = DateTimeOffset.UtcNow;
        var errorTime = startTime.AddMinutes(waitingMinutes);
        var isCoffeeTaken = false;

        while (!isCoffeeTaken && DateTimeOffset.UtcNow < errorTime)
        {
            isCoffeeTaken = await _coffeePresenceChecker.Check();

            if (!isCoffeeTaken)
                await Task.Delay(delay);
        }

        return isCoffeeTaken;
    }
    
    private async Task<Result<bool>> RecipeIsNotFoundError(CoffeeWasOrderedEvent form)
    {
        var errorCode = Guid.NewGuid();
        var machineId = _configuration.CurrentValue.MachineId;
        
        Log.Error("COFFEE MACHINE ERROR({0}): A recipe with id {1} can't be found in the machine configuration, the error code - {2}", 
            machineId, form.RecipeId, errorCode);
        
        await _kafkaProducer.Push(new OrderHasBeenFailedEvent(form.OrderId, errorCode));

        return false;
    }

    private async Task<Result<bool>> AmountCountingError((Guid IngredientId, Result<int> Amount)[] results, CoffeeWasOrderedEvent form, bool shouldCommit)
    {
        var errorCode = Guid.NewGuid();
        var machineId = _configuration.CurrentValue.MachineId;

        foreach (var (ingredientId, amount) in results.Where(u => u.Amount.IsFailure))
        {
            Log.Error("COFFEE MACHINE ERROR({0}): error during counting a ingredient {1}, the error - {2}, the error code - {3}",
                machineId, ingredientId, amount.Error, errorCode);
        }

        await _kafkaProducer.Push(new OrderHasBeenFailedEvent(form.OrderId, errorCode));

        return shouldCommit;
    }
}