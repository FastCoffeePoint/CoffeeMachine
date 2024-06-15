using System.Collections.Immutable;
using Cmb.Application.Services;
using Cmb.Domain;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;

namespace Cmb.Application.Sensors.Fakes;

public class FakeRecipesSensor(IOptionsMonitor<CoffeeMachineConfiguration> config, IngredientsService ingredientsService) : IRecipesSensor
{
    private readonly TimeSpan Delay = new(0, 0, 0, 20);
    
    public async Task<Result> StartCooking(string sensorId, ImmutableList<OrderedCoffeeIngredientForm> ingredients)
    {
        if(config.CurrentValue.Recipes.All(u => u.SensorId != sensorId))
            return Result.Failure($"COFFEE MACHINE ERROR({config.CurrentValue.MachineId}): can't find a sensor with a id {sensorId}");
        
        await Task.Delay(Delay);


        var decreasingResults = new List<Result>();
        foreach (var ingredient in ingredients)
        {
            var decreasing = await ingredientsService.UseIngredient(new UseIngredientForm(ingredient.Id, ingredient.Amount));
            decreasingResults.Add(decreasing);
        }

        if (decreasingResults.All(u => u.IsSuccess))
            return Result.Success();

        return Result.Failure(string.Join(", ", decreasingResults.Where(u => u.IsFailure).Select(u => u.Error)));
    }
}