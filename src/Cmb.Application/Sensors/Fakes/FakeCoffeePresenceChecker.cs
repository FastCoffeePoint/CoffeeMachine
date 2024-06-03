namespace Cmb.Application.Sensors.Fakes;

public class FakeCoffeePresenceChecker : ICoffeePresenceChecker
{
    public bool Check()
    {
        return true;
    }
}