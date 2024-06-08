

namespace Cmb.Application.Sensors.Fakes;

//TODO: Test it
public class FakeCoffeePresenceChecker : ICoffeePresenceChecker
{
    private readonly TimeSpan Delay = new(0, 0, 0, 2);
    
    private readonly Timer Timer;
    private object locker = new();
    private bool State = true;


    public FakeCoffeePresenceChecker()
    {
        Timer = new(callback: TimerTask, state: 0, dueTime: 1000, period: 2000);
    }
    
    public async Task<bool> Check()
    {
        var result = true;

        lock (locker)
        {
            result = State;
        }
        
        return result;
    }
    
    private void TimerTask(object timerState)
    {
        lock (locker)
        {
            State = false;
        }
        
        Thread.Sleep(Delay);
        
        lock (locker)
        {
            State = true;
        }
    }
    
}