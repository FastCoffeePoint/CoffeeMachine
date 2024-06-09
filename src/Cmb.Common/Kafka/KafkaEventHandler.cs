using Serilog;

namespace Cmb.Common.Kafka;

public abstract class KafkaEventHandler<T> where T : IEvent
{
    /// <returns>Should commit</returns>
    public abstract Task<bool> Handle(T form);

    protected void LogHandlerError(T form, string error) =>
        Log.Error("KAFKA HANDLER {0} ERROR: {1}, DATA: {2}", GetType(), error, form);
}
