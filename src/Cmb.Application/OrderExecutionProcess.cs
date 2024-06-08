using System.Collections.Immutable;
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
    public async Task<(Result Result, bool ShouldCommit)> Execute(CoffeeWasOrderedEvent form)
    {
        //TODO: check a coffee presence

        var ingredientsBeforeExecution = await CountIngredient(form);
        if (ingredientsBeforeExecution.IsFailure)
            return (ingredientsBeforeExecution, false);

        var canOrderBeExecuted = CanOrderBeExecuted(form.Ingredients, ingredientsBeforeExecution.Value);
        if(!canOrderBeExecuted)
            return (Result.Success(), false);
        
        var recipeSensor = _configuration.CurrentValue.Recipes.FirstOrDefault(u => u.RecipeId == form.RecipeId);
        if (recipeSensor == null)
            return (await RecipeIsNotFoundError(form), false);
            
        await _kafkaProducer.Push(new CoffeeStartedBrewingEvent(form.OrderId, _configuration.CurrentValue.MachineId));
        await _recipesSensor.StartCooking(recipeSensor.SensorId);
        
        var ingredientsAfterExecution = await CountIngredient(form);
        if (ingredientsAfterExecution.IsFailure)
            return (ingredientsAfterExecution, true);

        var isCoffeeTaken = await WaitUntilCoffeeWillBeTaken(5); //TODO: move 5 to config
        //TODO: if a coffee wasn't taken probably alert? Or infinite waiting?
        
        return (Result.Success(), true);
    }

    private bool CanOrderBeExecuted(ImmutableList<OrderedCoffeeIngredientForm> orderIngredients, ImmutableList<CoffeeMachineIngredient> machineIngredients)
    {
        var errorCode = Guid.NewGuid();
        var machineId = _configuration.CurrentValue.MachineId;
        var canOrderBeExecuted = true;
        
        foreach (var orderIngredient in orderIngredients)
        {
            var machineIngredient = machineIngredients.FirstOrDefault(u => orderIngredient.Id == u.Id);
            if (machineIngredient == null)
            {
                Log.Error("COFFEE MACHINE ERROR({0}): An ingredient {1} can't be found, the error code - {2}",
                    machineId, orderIngredient.Id, errorCode);
                continue;
            }

            if (orderIngredient.Amount > machineIngredient.Amount)
                canOrderBeExecuted = false;
        }

        return canOrderBeExecuted;
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

    private async Task<Result<ImmutableList<CoffeeMachineIngredient>>> CountIngredient(CoffeeWasOrderedEvent form)
    {
        var ingredients = _configuration.CurrentValue.Ingredients;
        var amountTasksBeforeExecution = ingredients.Select(async u => (u.IngredientId, Amount: await _ingredientsSensor.GetAmount(u.SensorId)));
        var amountResultsBeforeExecution = await Task.WhenAll(amountTasksBeforeExecution);
        if (amountResultsBeforeExecution.Any(u => u.Amount.IsFailure))
        {
            var errorCode = await AmountCountingError(amountResultsBeforeExecution, form);
            return Result.Failure<ImmutableList<CoffeeMachineIngredient>>($"Can't calculate ingredients in a machine: error code - {errorCode}");
        }

        return amountResultsBeforeExecution.Select(u => new CoffeeMachineIngredient(u.IngredientId, u.Amount.Value))
            .ToImmutableList();
    }

    private async Task<Guid> AmountCountingError((Guid IngredientId, Result<int, string> Amount)[] results, CoffeeWasOrderedEvent form)
    {
        var errorCode = Guid.NewGuid();
        var machineId = _configuration.CurrentValue.MachineId;

        foreach (var (ingredientId, amount) in results.Where(u => u.Amount.IsFailure))
        {
            Log.Error("COFFEE MACHINE ERROR({0}): error during counting a ingredient {1}, the error - {2}, the error code - {3}",
                machineId, ingredientId, amount.Error, errorCode);
        }

        await _kafkaProducer.Push(new OrderHasBeenFailedEvent(form.OrderId, errorCode));
        return errorCode;
    }
}