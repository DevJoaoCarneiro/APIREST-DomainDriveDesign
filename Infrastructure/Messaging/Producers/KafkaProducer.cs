using Application.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Infrastructure.Messaging.Producers
{
    public class KafkaProducer : IEventProducer
    {
        private readonly IConfiguration _configuration;
        private readonly IProducer<Null, string> _producer;

        public KafkaProducer(IConfiguration configuration)
        {
            _configuration = configuration;
            var config = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"]
            };

            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task PublishAsync<T>(T message)
        {
            var eventType = typeof(T).Name;
            var topic = _configuration[$"Kafka:Topics:PasswordReesend"];

            if (string.IsNullOrEmpty(topic))
            {
                throw new Exception($"Tópico não configurado para o evento '{eventType}' no appsettings.");
            }

            var json = JsonSerializer.Serialize(message);
            await _producer.ProduceAsync(topic, new Message<Null, string> { Value = json });
        }
    }
}
