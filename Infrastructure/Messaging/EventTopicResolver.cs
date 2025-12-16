using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Messaging
{
    public class EventTopicResolver : IEventTopicResolver
    {
        private readonly IConfiguration _configuration;

        public EventTopicResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Resolve<T>()
        {
            var eventName = typeof(T).Name;
            var topic = _configuration[$"Kafka:Topics:{eventName}"];

            if (string.IsNullOrEmpty(topic))
                throw new Exception($"Tópico Kafka não configurado para o evento {eventName}");

            return topic;
        }
    }
}
