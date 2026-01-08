using Reqnroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Tests.E2E.Drivers;
using Tests.E2E.Support;
using Xunit;

namespace Tests.E2E.StepDefinitions
{
    [Binding]
    public class UserRegistrationStepDefinitions
    {
        private readonly UserDriver _driver;
        private object? _requestData;
        private readonly TestWebFactory _factory;

        public UserRegistrationStepDefinitions(TestWebFactory factory)
        {
            _factory = factory;
            var client = _factory.CreateClient();
            _driver = new UserDriver(client);
        }

        [Given("I provide the following user data:")]
        public void GivenIProvideTheFollowingUserData(DataTable table)
        {
            var row = table.Rows[0];

            _requestData = new
            {
                Name = row["Name"],
                Mail = row["Mail"],
                password = row["Password"],
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
            await _driver.RegisterUser(_requestData!);
        }

        [Then("the response status code should be {int}")]
        public async Task ThenTheResponseStatusCodeShouldBe(int statusCode)
        {
            if (_driver.LastResponse == null)
                throw new Exception("API response is null.");

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
            if (_driver.LastResponse == null)
                throw new Exception("API response is null.");

            var content = await _driver.LastResponse.Content.ReadAsStringAsync();
            using var json = JsonDocument.Parse(content);

            var property = json.RootElement.EnumerateObject()
                .FirstOrDefault(p => string.Equals(p.Name, field, StringComparison.OrdinalIgnoreCase));

            if (property.Value.ValueKind == JsonValueKind.Undefined)
            {
                throw new KeyNotFoundException($"Property '{field}' not found in JSON response: {content}");
            }

            var actualValue = property.Value.GetString();
            Assert.Equal(expectedValue, actualValue);
        }
    }
}