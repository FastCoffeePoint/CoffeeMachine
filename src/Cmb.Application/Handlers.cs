using Cmb.Common.Kafka;
using Cmb.Domain;

namespace Cmb.Application;

public class CoffeeWasOrderedEventHandler() : KafkaEventHandler<CoffeeWasOrderedEvent>
{
    public override async Task Handle(CoffeeWasOrderedEvent form)
    {
        
    }
}