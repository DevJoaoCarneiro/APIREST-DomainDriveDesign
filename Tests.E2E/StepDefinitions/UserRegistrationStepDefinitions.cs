using Reqnroll;
using Reqnroll.BoDi;
using System.Text.Json;

using Xunit;

namespace Tests.E2E.StepDefinitions
{
    [Binding]
    public class UserRegistrationStepDefinitions
    {
        private readonly IObjectContainer _container;
        private UserDriver? _driver;
        private object? _requestData;

        public UserRegistrationStepDefinitions(IObjectContainer container)
        {
            _container = container;
        }

        [BeforeScenario]
        public void Setup()
        {
            var httpClient = _container.Resolve<HttpClient>();
            _driver = new UserDriver(httpClient);
        }

        [Given("I provide the following user data:")]
        public void GivenIProvideTheFollowingUserData(DataTable table)
        {
            var row = table.Rows[0];

            _requestData = new
            {
                Name = row["Name"],
                Mail = row["Mail"],
                Password = row["Password"],
                UserAddress = new
                {
                    Street = row["Street"],
                    Number = row["Number"],
                    City = row["City"],
                    State = row["State"],
                    ZipCode = row["ZipCode"]
                }
            };

        }

        [When("I send a POST request to {string}")]
        public async Task WhenISendAPostRequestTo(string endpoint)
        {
            if (_driver == null)
                throw new InvalidOperationException("Driver not initialized. Did you call Given step?");

            await _driver.RegisterUser(_requestData!);
        }

        [Then("the response status code should be {int}")]
        public async Task ThenTheResponseStatusCodeShouldBe(int statusCode)
        {
            if (_driver?.LastResponse == null)
                throw new InvalidOperationException("API response is null.");

            var actualStatusCode = (int)_driver.LastResponse.StatusCode;

            if (actualStatusCode != statusCode)
            {
                var errorContent = await _driver.LastResponse.Content.ReadAsStringAsync();
                throw new Exception($"Expected {statusCode} but received {actualStatusCode}. Details: {errorContent}");
            }

            Assert.Equal(statusCode, actualStatusCode);
        }

        [Then("the response body {string} should be {string}")]
        public async Task ThenTheResponseBodyShouldBe(string field, string expectedValue)
        {
            if (_driver?.LastResponse == null)
                throw new InvalidOperationException("API response is null.");

            var content = await _driver.LastResponse.Content.ReadAsStringAsync();

            using var json = JsonDocument.Parse(content);

            var property = json.RootElement
                .EnumerateObject()
                .FirstOrDefault(p => p.Name.Equals(field, StringComparison.OrdinalIgnoreCase));

            if (property.Value.ValueKind == JsonValueKind.Undefined)
            {
                throw new KeyNotFoundException(
                    $"Property '{field}' not found in JSON response: {content}");
            }

            var actualValue = property.Value.GetString();

            Assert.Equal(expectedValue, actualValue);
        }
    }
}
