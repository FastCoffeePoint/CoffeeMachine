namespace Cmb.Application.Sensors;

public interface IIngredientSensor
{
    Task<int> GetAmount(string sensorId);
}