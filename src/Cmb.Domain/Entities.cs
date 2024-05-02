using System.Collections.Immutable;

namespace Cmb.Domain;

public record CoffeeRecipe(Guid Id, string Name, ImmutableList<CoffeeRecipeIngredient> Ingredients);

public record CoffeeRecipeIngredient(Guid Id, string Name);

public record Ingredient(Guid Id, string Name, int Amount);