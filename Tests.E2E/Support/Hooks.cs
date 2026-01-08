using Reqnroll;
using Tests.E2E.Support;

namespace Tests.E2E.Support
{
    [Binding]
    public class Hooks
    {
        private readonly ScenarioContext _scenarioContext;

        public Hooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            var factory = new TestWebFactory();
            var client = factory.CreateClient();

            _scenarioContext.Set(factory);
            _scenarioContext.Set(client);
        }

        [AfterScenario]
        public void AfterScenario()
        {
            if (_scenarioContext.TryGetValue<TestWebFactory>(out var factory))
            {
                factory.Dispose();
            }
        }
    }
}
