using System.Collections.Immutable;
using Cmb.Database;
using Cmb.Database.Entities;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace Cmb.Application.Services;

public class IngredientsService(DbCoffeeMachineContext _dc)
{
    public async Task<ImmutableList<DbIngredient>> GetIngredients() =>
        (await _dc.Ingredients.ExcludeDeleted().AsNoTracking().ToListAsync()).ToImmutableList();

    public async Task<Result<Guid, string>> Create(CreateIngredientForm form)
    {
        if (string.IsNullOrEmpty(form.Name) && form.Name.Length > 3)
            return "Invalid name for ingredient";
        
        var nameIsBusy = await _dc.Ingredients.ExcludeDeleted().AnyAsync(u => u.Name == form.Name);
        if (nameIsBusy)
            return "The ingredient name is busy";

        var ingredient = new DbIngredient
        {
            Id = Guid.NewGuid(), 
            Name = form.Name
        }.MarkCreated();
        _dc.Ingredients.Add(ingredient);
        await _dc.SaveChangesAsync();

        return ingredient.Id;
    }

    public async Task<Result<Guid, string>> Delete(Guid ingredientId)
    {
        var ingredient = await _dc.Ingredients.FirstOrDefaultAsync(u => u.Id == ingredientId);
        if (ingredient == null)
            return "A ingredient is not found";
        if (ingredient.IsDeleted)
            return ingredientId;

        var anyCoffeeRecipeHasIngredient =  await _dc.CoffeeRecipeIngredients.AnyAsync(u => u.IngredientId == ingredientId);
        if (anyCoffeeRecipeHasIngredient)
            return "Any coffee recipe has the ingredient, so that you can't delete this.";

        ingredient.MarkDeleted();
        await _dc.SaveChangesAsync();

        return ingredientId;
    }

    public async Task<Result<Guid, string>> ReplenishIngredient(ReplenishIngredientForm form)
    {
        if (form.IncreaseAmount <= 0)
            return "Amount must be positive";
        
        var ingredient = await _dc.Ingredients.ExcludeDeleted().FirstOrDefaultAsync(u => u.Id == form.IngredientId);
        if (ingredient == null)
            return "A ingredient is not found";

        ingredient.Amount += form.IncreaseAmount;
        await _dc.SaveChangesAsync();

        return ingredient.Id;
    }
}