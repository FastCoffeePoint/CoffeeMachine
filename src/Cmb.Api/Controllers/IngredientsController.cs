using System.Collections.Immutable;
using Cmb.Api.AspNetCore;
using Cmb.Application;
using Cmb.Application.Services;
using Cmb.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Cmb.Api.Controllers;

public class IngredientsController(IngredientsService _ingredientsService) : CoffeePointController
{
    [HttpPost(DefaultUrl)]
    public async Task<JsonResult<Guid, string>> ReplenishIngredient(ReplenishIngredientForm form) => 
        await _ingredientsService.ReplenishIngredient(form);
    
    [HttpPost(DefaultUrl)]
    public async Task<JsonOptionError> UseIngredient(UseIngredientForm form) => 
        await _ingredientsService.UseIngredient(form);
    
    [HttpGet(DefaultUrl)]
    public async Task<ImmutableList<CoffeeMachineIngredient>> GetIngredients() => 
        await _ingredientsService.GetIngredients();
}