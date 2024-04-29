using System.Text;
using System.Text.Json;
using Confluent.Kafka;

namespace Cmb.Common.Kafka;

public class DefaultJsonSerializer<T> : ISerializer<T> where T : class
{
    public byte[] Serialize(T data, SerializationContext context) => 
        Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
}