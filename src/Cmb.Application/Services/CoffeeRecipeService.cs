using Cmb.Database;
using Cmb.Database.Entities;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace Cmb.Application.Services;

public class CoffeeService(DbCoffeeMachineContext _dc)
{
    public async Task<Result<Guid, string>> CreateCoffeeRecipe(CreateCoffeeRecipe form)
    {
        var isNameBusy = await _dc.CoffeeRecipes.ExcludeDeleted()
            .AnyAsync(u => u.DeleteDate == null && u.Name == form.Name);

        if (isNameBusy)
            return "The coffee name is busy";

        var coffeeRecipe = new DbCoffeeRecipe
        {
            Id = Guid.NewGuid(),
            Name = form.Name
        }.MarkCreated();
        
        _dc.CoffeeRecipes.Add(coffeeRecipe);
        await _dc.SaveChangesAsync();

        return coffeeRecipe.Id;
    }

    public async Task<Result<Guid, string>> DeleteCoffeeRecipe(Guid recipeId)
    {
        var recipe = await _dc.CoffeeRecipes.ExcludeDeleted().FirstOrDefaultAsync(u => u.Id == recipeId);
        if (recipe == null)
            return "A recipe is not found";
        if (recipe.IsDeleted)
            return recipeId;

        recipe.MarkDeleted();
        var linkEntities = await _dc.CoffeeRecipeIngredients
            .Where(u => u.CoffeeRecipeId == recipeId)
            .ToListAsync();
        _dc.CoffeeRecipeIngredients.RemoveRange(linkEntities);
        await _dc.SaveChangesAsync();

        return recipeId;
    }
}