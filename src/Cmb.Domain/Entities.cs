using System.Collections.Immutable;
using Cmb.Common.Kafka;

namespace Cmb.Domain;

// Coffee machine
public class CoffeeMachineConfiguration
{
    public Guid MachineId { get; init; }
    public List<ConfigurationIngredient> Ingredients { get; init; }
    public List<ConfigurationRecipe> Recipes { get; init; }
}
public record ConfigurationIngredient(Guid IngredientId, string SensorId);
public record ConfigurationRecipe(Guid RecipeId, string SensorId);

public record CoffeeMachineIngredient(Guid Id, int Amount);

// Orders
public record OrderedCoffeeIngredientForm(Guid Id, int Amount);
public record CoffeeWasOrderedEvent(Guid OrderId, Guid RecipeId, ImmutableList<OrderedCoffeeIngredientForm> Ingredients): IEvent
{
    public static string Name => "CoffeeWasOrderedEvent";
}
public record CoffeeStartedBrewingEvent(Guid OrderId, Guid MachineId): IEvent
{
    public static string Name => "CoffeeStartedBrewingEvent";
}
public record ExecutedCoffeeIngredientForm(Guid Id, int AmountBeforeExecution, int AmountAfterExecution);
public record CoffeeIsReadyToBeGottenEvent(Guid MachineId, Guid OrderId, ImmutableList<ExecutedCoffeeIngredientForm> Ingredients): IEvent
{
    public static string Name => "CoffeeIsReadyToBeGottenEvent";
}
public record OrderHasBeenCompletedEvent(Guid OrderId): IEvent
{
    public static string Name => "OrderHasBeenCompletedEvent";
}
public record OrderHasBeenFailedEvent(Guid OrderId, Guid ErrorCode): IEvent
{
    public static string Name => "OrderHasBeenFailedEvent";
}

