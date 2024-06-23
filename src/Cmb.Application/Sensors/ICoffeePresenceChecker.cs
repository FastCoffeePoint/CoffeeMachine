namespace Cmb.Application.Sensors;

public interface ICoffeePresenceChecker
{
    Task<bool> Check();
}