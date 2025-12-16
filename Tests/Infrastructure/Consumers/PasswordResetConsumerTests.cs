using Infrastructure.Messaging.Consumers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests.Infrastructure.Consumers
{
    public class PasswordResetConsumerTests
    {
        private readonly IConfiguration _configuration = Substitute.For<IConfiguration>();
        private readonly IServiceScopeFactory _serviceScopeFactory = Substitute.For<IServiceScopeFactory>();
        private readonly ILogger<PasswordResetConsumer> _logger = Substitute.For<ILogger<PasswordResetConsumer>>();
        private readonly PasswordResetConsumer _consumer;
        private string _topic;

        public PasswordResetConsumerTests()
        {
            _consumer = new PasswordResetConsumer(
                _configuration,
                _serviceScopeFactory,
                _logger);
        }
    }
}
