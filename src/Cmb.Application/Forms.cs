namespace Cmb.Application;

// Ingredient
public record ReplenishIngredientForm(Guid IngredientId, int IncreaseAmount);

public record UseIngredientForm(Guid IngredientId, int DecreaseAmount);
