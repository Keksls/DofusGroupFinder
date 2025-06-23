using Zaapix.Domain.DTO.Requests;
using System.Net.Http.Json;

namespace Zaapix.Client.Services
{
    public class PresenceClient
    {
        public async Task ConnectAsync() => await HttpClientFactory.Instance.PostAsync("api/presence/connect", null);
        public async Task DisconnectAsync() => await HttpClientFactory.Instance.PostAsync("api/presence/disconnect", null);
        public async Task PingAsync(bool? isInGame = null, bool? isInGroup = null)
        {
            PresenceUpdateRequest request = new PresenceUpdateRequest()
            {
                IsInGame = isInGame,
                IsInGroup = isInGroup
            };
            await HttpClientFactory.Instance.PostAsJsonAsync("api/presence/ping", request);
        }
    }
}