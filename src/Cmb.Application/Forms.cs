namespace Cmb.Application;

// Ingredient
public record CreateIngredientForm(string Name);

public record ReplenishIngredientForm(Guid IngredientId, int IncreaseAmount);


// Coffee recipe
public record CreateCoffeeRecipe(string Name);

public record ManageIngredientInRecipeForm(Guid RecipeId, Guid IngredientId);