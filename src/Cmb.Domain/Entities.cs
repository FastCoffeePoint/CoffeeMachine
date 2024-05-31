using System.Collections.Immutable;
using Cmb.Common;
using Cmb.Common.Kafka;

namespace Cmb.Domain;

public record CoffeeRecipe(Guid Id, string Name, ImmutableList<CoffeeRecipeIngredient> Ingredients);

public record CoffeeRecipeIngredient(Guid Id, string Name);

public record Ingredient(Guid Id, string Name, int Amount);

// Orders
public record CoffeeMachineIngredientForm(Guid Id, int AmountBeforeExecution, int AmountAfterExecution);
public record CoffeeWasOrderedEvent(Guid OrderId, Guid RecipeId): IEvent
{
    public static string Name => "CoffeeWasOrderedEvent";
}
public record CoffeeStartedBrewingEvent(Guid OrderId): IEvent
{
    public static string Name => "CoffeeStartedBrewingEvent";
}
public record CoffeeIsReadyToBeGottenEvent(Guid MachineId, Guid OrderId, ImmutableList<CoffeeMachineIngredientForm> Ingredients): IEvent
{
    public static string Name => "CoffeeIsReadyToBeGottenEvent";
}
public record OrderHasBeenCompletedEvent(Guid OrderId): IEvent
{
    public static string Name => "OrderHasBeenCompletedEvent";
}

// Coffee machine
public record CoffeeMachineConfiguration(Guid MachineId, ImmutableList<ConfigurationIngredient> Ingredients, ImmutableList<ConfigurationRecipe> Recipes);

public record ConfigurationIngredient(Guid IngredientId, string SensorId);
public record ConfigurationRecipe(Guid RecipeId, string SensorId);