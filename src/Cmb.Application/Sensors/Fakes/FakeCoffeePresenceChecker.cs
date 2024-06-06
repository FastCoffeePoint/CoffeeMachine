namespace Cmb.Application.Sensors.Fakes;

public class FakeCoffeePresenceChecker : ICoffeePresenceChecker
{
    public async Task<bool> Check()
    {
        return true;
    }
}