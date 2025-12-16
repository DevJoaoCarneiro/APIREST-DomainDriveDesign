using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IEventProducer
    {
        Task PublishAsync<T>(T message);
    }
}
