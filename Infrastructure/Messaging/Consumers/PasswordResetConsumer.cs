using Application.Interfaces;
using Application.Request;
using Confluent.Kafka;
using Domain.entities;
using Infrastructure.Messaging.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Messaging.Consumers
{
    public class PasswordResetConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<PasswordResetConsumer> _logger;
        private string _topic;

        public PasswordResetConsumer(
            IConfiguration configuration,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<PasswordResetConsumer> logger)
        {
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _topic = _configuration[$"Kafka:Topics:ResetRequestEventDTO"];
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
                GroupId = "password-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(_topic);
            _logger.LogInformation($"[Kafka Consumer] Iniciado. Ouvindo tópico: {_topic}");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);

                    if (string.IsNullOrWhiteSpace(result?.Message?.Value))
                        continue;

                    using var scope = _serviceScopeFactory.CreateScope();
                    var useCase = scope.ServiceProvider
                        .GetRequiredService<IHandlePasswordReset>();

                    var dto = JsonSerializer.Deserialize<ResetRequestEventDTO>(
                        result.Message.Value
                    );

                    if (dto is null)
                    {
                        _logger.LogWarning("Evento inválido.");
                        continue;
                    }

                    await useCase.HandleAsync(dto);

                    consumer.Commit(result);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError($"Erro Kafka: {ex.Error.Reason}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro inesperado: {ex.Message}");
                    await Task.Delay(1000, stoppingToken);
                }
            }

            consumer.Close();
        }

       
    }
}
