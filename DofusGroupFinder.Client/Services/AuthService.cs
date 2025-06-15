using System.IO;
using System.Net.Http.Json;
using System.Text.Json;

namespace DofusGroupFinder.Client.Services
{
    public class AuthService
    {
        private const string FilePath = "Config/auth.json";

        public async Task<string?> LoginAsync(string email, string password)
        {
            var request = new { email = email, password = password };

            var response = await HttpClientFactory.Instance.PostAsJsonAsync("api/auth/login", request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result?.Token != null)
                {
                    SaveToken(result.Token);
                    return result.Token;
                }
            }
            return null;
        }

        public async Task<bool> RegisterAsync(string email, string password)
        {
            var request = new { email = email, password = password };
            var response = await HttpClientFactory.Instance.PostAsJsonAsync("api/auth/register", request);
            return response.IsSuccessStatusCode;
        }

        public void SaveToken(string token)
        {
            Directory.CreateDirectory("Config");
            File.WriteAllText(FilePath, JsonSerializer.Serialize(new AuthObject { Token = token }));
        }

        public string? LoadToken()
        {
            if (!File.Exists(FilePath))
                return null;

            var json = File.ReadAllText(FilePath);
            var obj = JsonSerializer.Deserialize<AuthObject>(json);
            return obj?.Token;
        }

        private class LoginResponse
        {
            public string Token { get; set; } = string.Empty;
        }

        private class AuthObject
        {
            public string Token { get; set; } = string.Empty;
        }
    }
}