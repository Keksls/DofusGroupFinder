using DofusGroupFinder.Client.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace DofusGroupFinder.Client.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiClient(string jwtToken)
        {
            _httpClient = HttpClientFactory.Instance;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        public void SetJwtToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<T?> GetAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

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
            response.EnsureSuccessStatusCode();

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
            response.EnsureSuccessStatusCode();
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

        // Ensuite tu gardes tous tes appels spécifiques exactement comme tu les avais

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

        public async Task<List<PublicListingResponse>?> GetPublicListingsAsync()
            => await GetAsync<List<PublicListingResponse>>("api/public/listings");

        public async Task<List<PublicListingResponse>?> SearchPublicListingsAsync(Guid? dungeonId = null, int? minRemainingSlots = null, bool? wantSuccess  = null)
        {
            var query = new List<string>();

            if (dungeonId.HasValue)
                query.Add($"dungeonId={dungeonId}");

            if (minRemainingSlots.HasValue)
                query.Add($"minRemainingSlots={minRemainingSlots}");

            if (wantSuccess.HasValue)
                query.Add($"wantSuccess={wantSuccess}");

            var server = App.SettingsService.LoadServer();
            if (server == null)
                return null;

            var url = $"api/public/listings/search?server={server}";
            if (query.Count > 0)
                url += "&" + string.Join("&", query);

            return await GetAsync<List<PublicListingResponse>>(url);
        }

        public async Task<List<GroupMemberResponse>?> GetGroupMembersAsync(Guid listingId)
            => await GetAsync<List<GroupMemberResponse>>($"api/listings/{listingId}/members");

        public async Task<GroupMemberResponse?> AddGroupMemberAsync(Guid listingId, CreateGroupMemberRequest request)
            => await PostAsync<GroupMemberResponse>($"api/listings/{listingId}/members", request);

        public async Task<GroupMemberResponse?> UpdateGroupMemberAsync(Guid listingId, Guid memberId, UpdateGroupMemberRequest request)
            => await PutAsync<GroupMemberResponse>($"api/listings/{listingId}/members/{memberId}", request);

        public async Task DeleteGroupMemberAsync(Guid listingId, Guid memberId)
            => await DeleteAsync($"api/listings/{listingId}/members/{memberId}");

        public async Task UpdateCharacterAsync(Guid characterId, UpdateCharacterRequest request)
        {
            await PutAsync($"api/characters/{characterId}", request);
        }
    }
}