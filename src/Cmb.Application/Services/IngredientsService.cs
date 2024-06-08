using System.Collections.Immutable;
using Cmb.Database;
using Cmb.Database.Entities;
using Cmb.Domain;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace Cmb.Application.Services;

public class IngredientsService(DbCoffeeMachineContext _dc)
{
    public async Task Initiate(ImmutableList<(Guid IngredientId, string SensorId)> ingredientIds)
    {
        var ingredients = ingredientIds.Select(u => new DbFakeIngredient
        {
            Id = u.IngredientId,
            SensorId = u.SensorId,
            Amount = 0
        });

        _dc.Ingredients.AddRange(ingredients);
        await _dc.SaveChangesAsync();
    }

    public async Task<Result<Guid, string>> ReplenishIngredient(ReplenishIngredientForm form)
    {
        if (form.IncreaseAmount <= 0)
            return "Amount must be positive";
        
        var ingredient = await _dc.Ingredients.FirstOrDefaultAsync(u => u.Id == form.IngredientId);
        if (ingredient == null)
            return "A ingredient is not found";

        ingredient.Amount += form.IncreaseAmount;
        await _dc.SaveChangesAsync();

        return ingredient.Id;
    }

    public async Task<Result<int, string>> GetAmount(string sensorId)
    {
        var ingredient = await _dc.Ingredients.FirstOrDefaultAsync(u => u.SensorId == sensorId);
        if (ingredient == null)
            return $"An ingredient for sensor {sensorId} is not found";

        return ingredient.Amount;
    }

    public async Task<ImmutableList<CoffeeMachineIngredient>> GetIngredients() =>
        (await _dc.Ingredients.AsNoTracking().Select(u => new CoffeeMachineIngredient(u.Id, u.Amount)).ToListAsync()).ToImmutableList();
}