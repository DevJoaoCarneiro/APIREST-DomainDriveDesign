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

        public UserRegistrationStepDefinitions(TestWebFactory factory)
        {
            var client = factory.CreateClient();
            _driver = new UserDriver(client);
        }

        [Given(@"que eu informo os seguintes dados de usuário:")]
        public void GivenQueEuInformoOsSeguintesDados(Table table)
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

        [When(@"eu envio uma requisição POST para ""(.*)""")]
        public async Task WhenEuEnvioUmaRequisicaoPostPara(string endpoint)
        {
            await _driver.RegisterUser(_requestData!);
        }

        [Then(@"o status code da resposta deve ser (.*)")]
        public async Task ThenOStatusCodeDaRespostaDeveSer(int statusCode)
        {
            if ((int)_driver.LastResponse!.StatusCode != statusCode)
            {
                var errorContent = await _driver.LastResponse.Content.ReadAsStringAsync();
                throw new Exception($"Esperava status {statusCode} mas recebi {(int)_driver.LastResponse.StatusCode}. Detalhes: {errorContent}");
            }
            Assert.Equal(statusCode, (int)_driver.LastResponse.StatusCode);
        }

        [Then(@"o corpo da resposta ""(.*)"" deve ser ""(.*)""")]
        public async Task ThenOCorpoDaRespostaDeveSer(string campo, string valorEsperado)
        {
            var content = await _driver.LastResponse!.Content.ReadAsStringAsync();

            using var json = JsonDocument.Parse(content);

            var propriedade = json.RootElement.EnumerateObject()
                .FirstOrDefault(p => string.Equals(p.Name, campo, StringComparison.OrdinalIgnoreCase));

            if (propriedade.Value.ValueKind == JsonValueKind.Undefined)
            {
                throw new KeyNotFoundException($"A propriedade '{campo}' não foi encontrada no JSON de resposta: {content}");
            }

            var valorAtual = propriedade.Value.GetString();
            Assert.Equal(valorEsperado, valorAtual);
        }
    }
}