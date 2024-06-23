using Cmb.Common.Kafka;
using Cmb.Domain;

namespace Cmb.Application;

public class CoffeeWasOrderedEventHandler(OrderExecutionProcess orderExecutionProcess) : KafkaEventHandler<CoffeeWasOrderedEvent>
{
    protected override async Task<bool> Handle(CoffeeWasOrderedEvent form)
    {
        var (result, shouldCommit) = await orderExecutionProcess.Execute(form);
        
        if(result.IsFailure)
            LogHandlerError(form, result.Error);

        return shouldCommit;
    }
}