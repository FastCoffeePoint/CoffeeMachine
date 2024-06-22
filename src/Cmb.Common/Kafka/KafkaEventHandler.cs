using System.Text.Json;
using Serilog;

namespace Cmb.Common.Kafka;


public interface IKafkaEventHandler
{ 
    Task<bool> HandleRaw(string form);
}


public abstract class KafkaEventHandler<T> : IKafkaEventHandler where T : IEvent
{
    public Task<bool> HandleRaw(string form) => Handle(JsonSerializer.Deserialize<T>(form));

    protected abstract Task<bool> Handle(T from);

    protected void LogHandlerError(T form, string error) =>
        Log.Error("KAFKA HANDLER {0} ERROR: {1}, DATA: {2}", GetType(), error, form);
}
