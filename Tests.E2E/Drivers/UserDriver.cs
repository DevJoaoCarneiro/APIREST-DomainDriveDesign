using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Tests.E2E.Drivers
{
    public class UserDriver
    {
    
        private readonly HttpClient _client;
        public HttpResponseMessage? LastResponse { get; private set; }

        public UserDriver(HttpClient client)
        {
            _client = client;
        }

        public async Task RegisterUser(object userRequest)
        {
            LastResponse = await _client.PostAsJsonAsync("/api/users", userRequest);
        }
    }
}
