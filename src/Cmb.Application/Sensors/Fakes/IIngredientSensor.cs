namespace Cmb.Application.Sensors.Fakes;

public class FakeIngredientSensor : IIngredientSensor
{
    public async Task<int> GetAmount(string sensorId)
    {
        return 0;
    }
}