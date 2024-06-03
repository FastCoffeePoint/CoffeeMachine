using Cmb.Common.Kafka;
using Cmb.Domain;

namespace Cmb.Application;

public class CoffeeWasOrderedEventHandler(OrderExecutionProcess orderExecutionProcess) : KafkaEventHandler<CoffeeWasOrderedEvent>
{
    public override async Task<bool> Handle(CoffeeWasOrderedEvent form)
    {
        var result = await orderExecutionProcess.Execute(form);
        if(result.IsSuccess)
            return true;
        
        LogHandlerError(form, result.Error);

        return result.Value;
    }
}