using Infrastructure.Context;
using Microsoft.Extensions.DependencyInjection;
using Reqnroll;
using Reqnroll.BoDi;
using Tests.E2E.Support;

namespace Tests.E2E.Support
{
    [Binding]
    public class Hooks
    {
        private readonly IObjectContainer _container;

        public Hooks(IObjectContainer container)
        {
            _container = container;
        }

        [BeforeScenario(Order = 0)]
        public void BeforeScenario()
        {
            var factory = new TestWebFactory();
            var client = factory.CreateClient();

            using var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();

            _container.RegisterInstanceAs(client);
        }
    }
}
