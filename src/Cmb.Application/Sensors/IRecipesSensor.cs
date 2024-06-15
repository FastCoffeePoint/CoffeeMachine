using System.Collections.Immutable;
using Cmb.Domain;
using CSharpFunctionalExtensions;

namespace Cmb.Application.Sensors;

public interface IRecipesSensor
{
    Task<Result> StartCooking(string sensorId, ImmutableList<OrderedCoffeeIngredientForm> ingredients);
}