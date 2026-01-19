using Application.Interfaces;
using Application.Request;
using Application.Response;
using Application.Services;
using Reqnroll;
using Xunit;

namespace Tests.E2E.StepDefinitions
{
    [Binding]
    public class UsuarioSteps
    {
        private readonly IUserServices _userService;
        private UserRequestDTO _userRequest;
        private UserResponseDTO _userResponse;

        public UsuarioSteps(IUserServices userService)
        {
            _userService = userService;
            _userRequest = new UserRequestDTO();
        }

        [Given(@"que eu tenho os seguintes dados do usuario:")]
        public void GivenQueEuTenhoOsSeguintesDadosDoUsuario(DataTable table)
        {
            var row = table.Rows[0];

            _userRequest.Name = row["Name"];
            _userRequest.Mail = row["Mail"];
            _userRequest.password = row["Password"];

            _userRequest.UserAddress = new AddressRequestDTO
            {
                Street = "Rua de Teste",
                Number = "123",
                City = "Cidade",
                State = "ST",
                ZipCode = "12345"
            };
        }

        [Given("que meu payload de usuario esta nulo")]
        public void GivenQueEuTenhoOsSeguintesDadosDoUsuarioNulosOuVazio()
        {
            _userRequest = null;
        }

        [When(@"eu envio os dados para cadastro")]
        public async Task WhenEuEnvioOsDadosParaCadastro()
        {
            _userResponse = await _userService.createUser(_userRequest);
        }

        [Then(@"o status da resposta deve ser (.*)")]
        public void ThenOStatusDaRespostaDeveSer(int statusCode)
        {
            if (statusCode == 200)
                Assert.Equal("Success", _userResponse.Status);

            if (statusCode == 400)
                Assert.Equal("invalid_argument", _userResponse.Status);
        }

        [Then(@"a mensagem deve ser ""(.*)""")]
        public void ThenAMensagemDeveSer(string mensagem)
        {
            Assert.Equal(mensagem, _userResponse.Message);
        }

        [Then(@"o campo ""(.*)"" deve ser ""(.*)""")]
        public void ThenOCampoDeveSer(string campo, string valor)
        {
            if (campo == "Status")
            {
                Assert.Equal(valor, _userResponse.Status);
            }
        }
    }
}