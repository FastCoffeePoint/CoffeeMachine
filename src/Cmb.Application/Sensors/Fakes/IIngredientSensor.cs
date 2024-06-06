using CSharpFunctionalExtensions;

namespace Cmb.Application.Sensors.Fakes;

public class FakeIngredientsSensor : IIngredientsSensor
{
    public async Task<Result<int>> GetAmount(string sensorId)
    {
        return 0;
    }
}