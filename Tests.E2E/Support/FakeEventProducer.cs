using Application.Interfaces;
using Infrastructure.Messaging;

namespace Tests.E2E.Support;

public class FakeEventProducer : IEventProducer
{
    public Task PublishAsync<T>(string topic, T message)
    {
        return Task.CompletedTask;
    }
}
