using Cmb.Application.Services;
using CSharpFunctionalExtensions;

namespace Cmb.Application.Sensors.Fakes;

public class FakeIngredientsSensor(IngredientsService _ingredientsService) : IIngredientsSensor
{
    private readonly TimeSpan Delay = new(0, 0, 0, 1);
    
    public async Task<Result<int, string>> GetAmount(string sensorId)
    {
        var amount = await _ingredientsService.GetAmount(sensorId);
        await Task.Delay(Delay);
        return amount;
    }
}