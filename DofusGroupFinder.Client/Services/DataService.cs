using DofusGroupFinder.Client.Models;

namespace DofusGroupFinder.Client.Services
{
    public class DataService
    {
        public Dictionary<Guid, DungeonResponse> Dungeons = new Dictionary<Guid, DungeonResponse>();
        public event Action? OnGetStaticData;

        /// <summary>
        /// Retreive static data from the API, such as dungeons.
        /// </summary>
        public async Task RetreiveStaticData()
        {
            List<DungeonResponse>? response = await App.ApiClient.GetAsync<List<DungeonResponse>>("api/dungeons");
            if (response == null)
            {
                response = new List<DungeonResponse>();
            }
            Dungeons = new Dictionary<Guid, DungeonResponse>();
            foreach (var dungeon in response)
            {
                Dungeons[dungeon.Id] = dungeon;
            }
            // Fire the event to notify that static data has been loaded
            OnGetStaticData?.Invoke();
        }
    }
}