using System.Collections.Immutable;
using Cmb.Api.AspNetCore;
using Cmb.Application;
using Cmb.Application.Services;
using Cmb.Database.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Cmb.Api.Controllers;

public class IngredientsController(IngredientsService _ingredientsService) : CoffeePointController
{
    [HttpPost(DefaultUrl)]
    public async Task<JsonResult<Guid, string>> CreateIngredient(CreateIngredientForm form) => 
        await _ingredientsService.Create(form);
    
    [HttpGet(DefaultUrl)]
    public async Task<ImmutableList<DbIngredient>> GetIngredients() => 
        await _ingredientsService.GetIngredients();
    
    [HttpPost(DefaultUrl)]
    public async Task<JsonResult<Guid, string>> DeleteIngredient(Guid ingredientId) => 
        await _ingredientsService.Delete(ingredientId);
    
    [HttpPost(DefaultUrl)]
    public async Task<JsonResult<Guid, string>> ReplenishIngredient(ReplenishIngredientForm form) => 
        await _ingredientsService.ReplenishIngredient(form);
}