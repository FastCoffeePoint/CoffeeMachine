namespace Cmb.Application.Sensors;

public interface IRecipesSensor
{
    Task StartCooking(string sensorId);
}