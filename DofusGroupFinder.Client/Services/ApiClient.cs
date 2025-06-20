using DofusGroupFinder.Domain.DTO;
using DofusGroupFinder.Domain.DTO.Requests;
using DofusGroupFinder.Domain.DTO.Responses;
using DofusGroupFinder.Domain.Entities;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace DofusGroupFinder.Client.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiClient()
        {
            _httpClient = HttpClientFactory.Instance;
        }

        public void SetJwtToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<T?> GetAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode == false)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                NotificationManager.ShowNotification(errorContent);
                return default(T);
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

        private async Task<T?> PostAsync<T>(string url, object body)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };
            var jsonBody = JsonConvert.SerializeObject(body, settings);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            if (response.IsSuccessStatusCode == false)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                NotificationManager.ShowNotification(errorContent);
                return default(T);
            }
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

        private async Task PutAsync(string url, object body)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };
            var jsonBody = JsonConvert.SerializeObject(body, settings);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(url, content);
            if (response.IsSuccessStatusCode == false)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                NotificationManager.ShowNotification(errorContent);
            }
        }

        private async Task<T?> PutAsync<T>(string url, object body)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };
            var jsonBody = JsonConvert.SerializeObject(body, settings);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(url, content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

        private async Task DeleteAsync(string url)
        {
            var response = await _httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<Character>?> GetCharactersAsync()
            => await GetAsync<List<Character>>("api/characters");

        public async Task<Character?> CreateCharacterAsync(CreateCharacterRequest request)
            => await PostAsync<Character>("api/characters", request);

        public async Task DeleteCharacterAsync(Guid characterId)
            => await DeleteAsync($"api/characters/{characterId}");

        public async Task<List<PublicListingResponse>?> GetMyListingsAsync()
            => await GetAsync<List<PublicListingResponse>>("api/listings");

        public async Task<PublicListingResponse?> CreateListingAsync(CreateListingRequest request)
            => await PostAsync<PublicListingResponse>("api/listings", request);

        public async Task UpdateListingAsync(Guid listingId, UpdateListingRequest request)
            => await PutAsync($"api/listings/{listingId}", request);

        public async Task DeleteListingAsync(Guid listingId)
            => await DeleteAsync($"api/listings/{listingId}");

        public async Task<List<PublicListingResponse>?> SearchPublicListingsAsync(Guid? dungeonId = null, int? minRemainingSlots = null, SuccesWantedState[] wantSuccess = null)
        {
            var query = new List<string>();

            if (dungeonId.HasValue)
                query.Add($"dungeonId={dungeonId}");

            if (minRemainingSlots.HasValue)
                query.Add($"minRemainingSlots={minRemainingSlots}");

            if (wantSuccess != null)
            {
                foreach (var success in wantSuccess)
                {
                    query.Add($"wantSuccess={success}");
                }
            }

            var server = App.SettingsService.LoadServer();
            if (server == null)
                return null;

            var url = $"api/public/listings/search?server={server}";
            if (query.Count > 0)
                url += "&" + string.Join("&", query);

            var result = await GetAsync<List<PublicListingResponse>>(url);

            // order result by success filter score
            if (wantSuccess != null)
                result = result.OrderByDescending(l => GetSuccessFilterScore(l.SuccessWanted, wantSuccess)).ToList();

            return result;
        }

        private int GetSuccessFilterScore(SuccesWantedState[] listing, SuccesWantedState[] request)
        {
            int score = 0;

            if (listing.Length != request.Length) // this should NEVER append
                return score;

            for (int i = 0; i < request.Length; i++)
            {
                var reqFilter = request[i];
                var listFilter = listing[i];
                if (reqFilter == SuccesWantedState.Osef || listFilter == SuccesWantedState.Osef)
                    continue;

                if (reqFilter == listFilter)
                    score++;
            }

            return score;
        }

        public async Task UpdateCharacterAsync(Guid characterId, UpdateCharacterRequest request)
        {
            await PutAsync($"api/characters/{characterId}", request);
        }

        #region Groups
        public async Task<List<Character>?> GetGroupMembersAsync(Guid listingId)
            => await GetAsync<List<Character>>($"api/group/{listingId}");

        public async Task AddGroupMemberAsync(Guid listingId, GroupMemberRequest request)
            => await PostAsync<object>($"api/group/{listingId}", request);

        public async Task RemoveGroupMemberAsync(Guid listingId, string groupMemberName)
            => await DeleteAsync($"api/group/{listingId}/{groupMemberName}");

        public async Task<bool> IsCharacterInGroupAsync(Guid characterId)
        {
            var result = await GetAsync<bool>($"api/group/is-in-group/{characterId}");
            return result;
        }

        public async Task DisbandGroupAsync(Guid listingId, bool deleteListing = false)
        {
            var url = $"api/group/{listingId}/disband?deleteListing={deleteListing.ToString().ToLower()}";
            await PostAsync<object>(url, new { });
        }
        #endregion

        public async Task<List<PublicCharacterLite>?> SearchCharactersAsync(string server, string query)
        {
            var url = $"api/characters/search?server={Uri.EscapeDataString(server)}&query={Uri.EscapeDataString(query)}";
            return await GetAsync<List<PublicCharacterLite>>(url);
        }

        public async Task<PublicListingResponse?> GetPublicListingByIdAsync(Guid listingId)
        {
            return await GetAsync<PublicListingResponse>($"api/listings/{listingId}");
        }

        public async Task<UpdateInfo?> GetLatestVersionAsync()
        {
            return await GetAsync<UpdateInfo>("api/auth/version");
        }
    }

    public class UpdateInfo
    {
        public string Version { get; set; } = "";
        public string Url { get; set; } = "";
    }
}