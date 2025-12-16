using Application.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Infrastructure.Messaging.Producers
{
    public class KafkaProducer : IEventProducer
    {
        private readonly IProducer<Null, string> _producer;

        public KafkaProducer(IConfiguration configuration)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"]
            };

            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task PublishAsync<T>(string topic, T message) { 

            if (string.IsNullOrEmpty(topic))
            {
                throw new ArgumentException("Topic não pode ser vazio.");
            }

            var json = JsonSerializer.Serialize(message);
            await _producer.ProduceAsync(topic, new Message<Null, string> { Value = json });
        }
    }
}
