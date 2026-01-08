using Confluent.Kafka;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

public class UserDriver
{
    private readonly HttpClient _httpClient;
    public HttpResponseMessage? LastResponse { get; private set; }

    public UserDriver(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task RegisterUser(object userRequest)
    {
        var json = JsonSerializer.Serialize(userRequest);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        LastResponse = await _httpClient.PostAsync("/api/users", content);
    }
}