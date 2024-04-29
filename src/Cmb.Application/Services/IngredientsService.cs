using System.Collections.Immutable;
using Cmb.Database;
using Cmb.Database.Entities;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace Cmb.Application.Services;

public class IngredientsService(DbCoffeeMachineContext _dc)
{
    public async Task<ImmutableList<DbIngredient>> GetIngredients() =>
        (await _dc.Ingredients.Where(u => u.DeleteDate == null).AsNoTracking().ToListAsync()).ToImmutableList();

    public async Task<Result<int, string>> Create(CreateIngredientForm form)
    {
        if (string.IsNullOrEmpty(form.Name) && form.Name.Length > 3)
            return "Invalid name for ingredient";
        
        var nameIsBusy = await _dc.Ingredients.AnyAsync(u => u.Name == form.Name && u.DeleteDate == null);
        if (nameIsBusy)
            return "The ingredient name is busy";

        var ingredient = new DbIngredient { Name = form.Name }.MarkCreated();
        _dc.Ingredients.Add(ingredient);
        await _dc.SaveChangesAsync();

        return ingredient.Id;
    }

    public async Task<Result<int, string>> Delete(int ingredientId)
    {
        var ingredient = await _dc.Ingredients.FirstOrDefaultAsync(u => u.Id == ingredientId);
        if (ingredient == null)
            return "A ingredient is not found";

        ingredient.MarkDeleted();
        await _dc.SaveChangesAsync();

        return ingredientId;
    }
}