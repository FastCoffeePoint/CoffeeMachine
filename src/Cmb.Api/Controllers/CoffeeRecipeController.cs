using System.Collections.Immutable;
using Cmb.Api.AspNetCore;
using Cmb.Application;
using Cmb.Application.Services;
using Cmb.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Cmb.Api.Controllers;

public class CoffeeController(CoffeeRecipeService _coffeeRecipeService) : CoffeePointController
{
    [HttpPost(DefaultUrl)]
    public async Task<JsonResult<Guid, string>> CreateCoffeeRecipe(CreateCoffeeRecipe form) => 
        await _coffeeRecipeService.CreateCoffeeRecipe(form);
    
    [HttpPost(DefaultUrl)]
    public async Task<JsonResult<Guid, string>> DeleteCoffeeRecipe(Guid recipeId) => 
        await _coffeeRecipeService.DeleteCoffeeRecipe(recipeId);
    
    [HttpPost(DefaultUrl)]
    public async Task<JsonResult<Guid, string>> AddIngredientToRecipe(ManageIngredientInRecipeForm form) => 
        await _coffeeRecipeService.AddIngredientToRecipe(form);
    
    [HttpPost(DefaultUrl)]
    public async Task<JsonResult<Guid, string>> RemoveIngredientFromRecipe(ManageIngredientInRecipeForm form) => 
        await _coffeeRecipeService.RemoveIngredientFromRecipe(form); 
    
    [HttpGet(DefaultUrl)]
    public async Task<ImmutableList<CoffeeRecipe>> GetCoffeeRecipes() => 
        await _coffeeRecipeService.GetCoffeeRecipes(); 
}