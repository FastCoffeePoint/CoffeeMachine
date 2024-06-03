namespace Cmb.Application.Sensors;

public interface IRecipeSensor
{
    Task StartCooking(string sensorId);
}