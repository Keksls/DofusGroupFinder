using DofusGroupFinder.Domain.DTO.Responses;
using DofusGroupFinder.Shared;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DofusGroupFinder.Client.Services
{
    public class DataService
    {
        public Dictionary<Guid, DungeonResponse> Dungeons = new Dictionary<Guid, DungeonResponse>();
        public Dictionary<string, ChallengeData> Challenges = new Dictionary<string, ChallengeData>();

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

            // Load the static data into the application
            string challJson = File.ReadAllText("Config/challenges.json");
            if (!string.IsNullOrEmpty(challJson))
            {
                var challenges = System.Text.Json.JsonSerializer.Deserialize<List<ChallengeData>>(challJson);
                if (challenges != null)
                {
                    Challenges = new Dictionary<string, ChallengeData>();
                    foreach (var challenge in challenges)
                    {
                        Challenges[challenge.Name.Fr] = challenge;
                    }
                }
            }

            // Fire the event to notify that static data has been loaded
            App.Events.InvokeGetStaticData();
        }

        public ImageSource GetIconForSuccess(string successName)
        {
            if(successName == "Spécial")
            {
                var uri = new Uri("pack://application:,,,/Icons/SpecialSuccesIcon.png");
                return new BitmapImage(uri);
            }

            int id = GetChallenge(successName).IconId;
            string url = $@"https://api.dofusdb.fr/img/challenges/{id}.png";
            return new ImageSourceConverter().ConvertFromString(url) as ImageSource;
        }

        public ChallengeData GetChallenge(string challenge)
        {
            challenge = challenge.Trim().ToLowerInvariant();
            foreach (var kvp in Challenges)
            {
                if (kvp.Key.Trim().ToLowerInvariant() == challenge)
                {
                    return kvp.Value;
                }
            }

            return new ChallengeData
            {
                Id = 0,
                IconId = 0,
                Name = new LocalizedText { Fr = challenge + " (Inconnu)" },
                Description = new LocalizedText { Fr = "" }
            };
        }
    }
}