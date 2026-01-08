using System.Net.Http.Json;

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
        LastResponse = await _httpClient.PostAsJsonAsync("/api/users", userRequest);
    }
}