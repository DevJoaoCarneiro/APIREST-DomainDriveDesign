using Application.Interfaces;
using Application.Request;
using Confluent.Kafka;
using Domain.entities;
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
            _topic = _configuration[$"Kafka:Topics:PasswordReesend"];
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

            try
            {
                consumer.Subscribe(_topic);
                _logger.LogInformation($"[Kafka Consumer] Iniciado. Ouvindo tópico: {_topic}");

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var result = consumer.Consume(stoppingToken);

                        if (result != null && !string.IsNullOrEmpty(result.Message.Value))
                        {
                            _logger.LogInformation($"[Kafka Consumer] Mensagem recebida: {result.Message.Value}");

                            await ProcessMessageAsync(result.Message.Value);

                            consumer.Commit(result);
                        }
                    }
                    catch (ConsumeException e)
                    {
                        _logger.LogError($"Erro ao consumir mensagem Kafka: {e.Error.Reason}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Erro genérico no loop do consumidor: {ex.Message}");
                        await Task.Delay(1000, stoppingToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                consumer.Close();
            }

        }


        private async Task ProcessMessageAsync(string messageJson)
        {
            var eventData = JsonSerializer.Deserialize<ResetRequestEventDTO>(messageJson);

            if (eventData == null)
            {
                _logger.LogWarning("Mensagem recebida mas estava vazia ou inválida.");
                return;
            }

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var notifier = scope.ServiceProvider.GetRequiredService<IPasswordResetNotifier>();

                var user = new User
                {
                    Name = eventData.UserName,
                    Mail = eventData.UserEmail
                };

                await notifier.SendResetLinkAsync(user, eventData.Token);

                _logger.LogInformation($"[Sucesso] Email de recuperação enviado para {eventData.UserEmail}");
            }
        }
    }
}
