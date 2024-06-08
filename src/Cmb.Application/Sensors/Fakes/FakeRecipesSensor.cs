using Cmb.Domain;
using Microsoft.Extensions.Options;
using Serilog;

namespace Cmb.Application.Sensors.Fakes;

public class FakeRecipesSensor(IOptionsMonitor<CoffeeMachineConfiguration> config) : IRecipesSensor
{
    private readonly TimeSpan Delay = new(0, 0, 0, 20);
    
    public async Task StartCooking(string sensorId)
    {
        await Task.Delay(Delay);
        if(config.CurrentValue.Recipes.All(u => u.SensorId != sensorId))
            Log.Warning("COFFEE MACHINE ERROR({0}): can't find a sensor with a id {1}", config.CurrentValue.MachineId, sensorId);
    }
}