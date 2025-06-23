using Zaapix.Domain.Entities;
using Zaapix.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace Zaapix.Baker
{
    internal class SuccesBaker
    {
        // Ce script :
        // 1. Télécharge tous les donjons
        // 2. Télécharge tous les succès par tranche de niveau
        // 3. Télécharge les objectifs de tous les succès
        // 4. Lie les succès aux donjons via monsterId
        // 5. Exporte en JSON local les dungeons enrichis
        List<DungeonApiModel> dungeons = new List<DungeonApiModel>();
        List<Achievement> achievements = new List<Achievement>();
        List<Monster> monsters = new List<Monster>();
        Dictionary<int, (string, string[])> successPerDj = new Dictionary<int, (string, string[])>();
        public async Task Bake()
        {
            bool download = false;
            bool removeDuo = true;

            if (download)
            {
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // --- 1. Download Dungeons ---
                int skip = 0, limit = 50, total = 0;
                bool first = true;
                do
                {
                    var res = await httpClient.GetAsync($"https://api.dofusdb.fr/dungeons?$skip={skip}&$limit={limit}&lang=fr");
                    var json = await res.Content.ReadAsStringAsync();
                    var parsed = JsonConvert.DeserializeObject<DungeonApiResponse>(json);
                    if (parsed == null) break;
                    if (first) { total = parsed.Total; first = false; }
                    dungeons.AddRange(parsed.Data);
                    skip += limit;
                } while (skip < total);
                // distinct on name
                dungeons = dungeons
                    .GroupBy(d => d.Name.Fr)
                    .Select(g => g.First())
                    .ToList();
                string jsonDungeons = JsonConvert.SerializeObject(dungeons, Formatting.Indented);
                File.WriteAllText("dungeons.json", jsonDungeons);

                // --- 2. Download Achievements (donjon categories) ---
                int[] categories = [11, 12, 13, 14, 59];
                foreach (var cat in categories)
                {
                    skip = 0; first = true;
                    do
                    {
                        var res = await httpClient.GetAsync($"https://api.dofusdb.fr/achievements?$skip={skip}&categoryId={cat}&$limit={limit}&$populate=false&lang=fr");
                        var json = await res.Content.ReadAsStringAsync();
                        var parsed = JsonConvert.DeserializeObject<AchievementApiResponse>(json);
                        if (parsed == null) break;
                        if (first) { total = parsed.Total; first = false; }
                        achievements.AddRange(parsed.Data);
                        skip += limit;
                    } while (skip < total);
                }
                string jsonAchievements = JsonConvert.SerializeObject(achievements, Formatting.Indented);
                File.WriteAllText("achievements.json", jsonAchievements);

                // --- 3. Download Monsters in dungeons ---
                foreach (var dungeon in dungeons)
                {
                    var query = string.Join("", dungeon.Monsters.Select(id => $"&id[]={id}"));
                    var url = $"https://api.dofusdb.fr/monsters?$limit={dungeon.Monsters.Count}&$populate=false&$skip=0&lang=fr" + query;

                    var res = await httpClient.GetAsync(url);
                    var json = await res.Content.ReadAsStringAsync();
                    var parsed = JsonConvert.DeserializeObject<MonsterApiResponse>(json);
                    if (parsed == null)
                    {
                        Console.WriteLine($"Failed to fetch monsters for dungeon {dungeon.Id} ({dungeon.Name.Fr})");
                        continue;
                    }
                    monsters.AddRange(parsed.Data);
                }
                string jsonMonsters = JsonConvert.SerializeObject(monsters, Formatting.Indented);
                File.WriteAllText("monsters.json", jsonMonsters);

                // --- 4. Bake success per dungeons ---
                foreach (var dungeon in dungeons)
                {
                    List<string> success = GetSuccessForDungeon(dungeon.Id);
                    successPerDj[dungeon.Id] = (dungeon.Name.Fr, success.ToArray());
                }
                // export to JSON
                string jsonSuccess = JsonConvert.SerializeObject(successPerDj, Formatting.Indented);
                File.WriteAllText("success_per_dungeon.json", jsonSuccess);
            }
            else
            {
                // load from json files
                dungeons = JsonConvert.DeserializeObject<List<DungeonApiModel>>(File.ReadAllText("dungeons.json"));
                achievements = JsonConvert.DeserializeObject<List<Achievement>>(File.ReadAllText("achievements.json"));
                monsters = JsonConvert.DeserializeObject<List<Monster>>(File.ReadAllText("monsters.json"));
                successPerDj = JsonConvert.DeserializeObject<Dictionary<int, (string, string[])>>(File.ReadAllText("success_per_dungeon.json"));
            }

            if(removeDuo)
            {
                foreach(var s in successPerDj.Keys)
                {
                    string[] success = successPerDj[s].Item2.Where(v => v != "Duo").ToArray();
                    successPerDj[s] = (successPerDj[s].Item1, success);
                }
            }

            // DB config
            var connectionString = "Host=postgres;Port=5432;Database=ZaapixDb;Username=postgres;Password=DofusGroup123!";
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(connectionString);
            using var dbContext = new ApplicationDbContext(optionsBuilder.Options);

            // clear existing dungeons in the database
            await dbContext.Dungeons.ExecuteDeleteAsync();

            // Injection en base (après export)
            foreach (var dungeonApi in dungeons)
            {
                var existingDungeon = await dbContext.Dungeons.FirstOrDefaultAsync(d => d.ExtId == dungeonApi.Id);
                var boss = GetDungeonBoss(dungeonApi.Id, monsters).FirstOrDefault();
                dbContext.Dungeons.Add(new Dungeon
                {
                    Id = Guid.NewGuid(),
                    ExtId = dungeonApi.Id,
                    Name = dungeonApi.Name.Fr,
                    Level = dungeonApi.OptimalPlayerLevel,
                    Succes = successPerDj[dungeonApi.Id].Item2,
                    BossGfxId = boss?.gfxId ?? -1,
                    BossId = boss?.Id ?? -1,
                    BossName = boss?.name.Fr ?? string.Empty
                });
            }

            await dbContext.SaveChangesAsync();

            Console.WriteLine("Exported baked dungeons with success list ✅");
        }

        /// <summary>
        /// Get the boss monster for a given dungeon ID from the list of monsters.
        /// </summary>
        /// <param name="dungeonId"> The ID of the dungeon.</param>
        /// <param name="monsters"> The list of all monsters.</param>
        /// <returns> The boss monster if found, otherwise null.</returns>
        private List<Monster> GetDungeonBoss(int dungeonId, List<Monster> monsters)
        {
            // get dungeon object
            var dungeon = dungeons.FirstOrDefault(m => m.Id == dungeonId);
            if (dungeon == null)
            {
                Console.WriteLine($"Dungeon with ID {dungeonId} not found.");
                return null;
            }

            // get dungeon monsters
            var bosses = dungeon.Monsters.Select(id => monsters.FirstOrDefault(m => m.Id == id)).Where(m => m != null && m.isBoss && !m.isMiniBoss).ToList();

            // get the first monster that is a boss
            return bosses;
        }

        private List<string> GetSuccessForDungeon(int dungeonId)
        {
            List<Monster> boss = GetDungeonBoss(dungeonId, monsters);
            if (boss == null || boss.Count == 0)
            {
                Console.WriteLine($"No boss found for dungeon {dungeonId}. Cannot extract success types.");
                return new List<string>();
            }
            string bossName = boss.First().name.Fr;

            // get all achievements that have the boss name in their name
            var archievementsForBoss = achievements
                .Where(a => a.Name.Fr.Contains(bossName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (archievementsForBoss.Count == 0)
            {
                Console.WriteLine($"No achievements found for boss {bossName} (ID: {boss.First().Id}) in dungeon {dungeonId}.");
                return new List<string>();
            }

            // extract success types from the achievement names
            List<string> successTypes = archievementsForBoss
                .Select(a =>
                {
                    var match = Regex.Match(a.Name.Fr, @"\(([^)]+)\)");
                    return match.Success ? match.Groups[1].Value : string.Empty;
                })
                .Where(type => !string.IsNullOrEmpty(type))
                .Distinct()
                .ToList();
            if (successTypes.Count == 0)
            {
                Console.WriteLine($"No success types found for dungeon {dungeonId} ({bossName}).");
            }
            else
            {
                Console.WriteLine($"Found success types for dungeon {dungeonId} ({bossName}): {string.Join(", ", successTypes)}");
            }
            return successTypes;
        }

        public async Task BakeChallenges()
        {
            var httpClient = new HttpClient();
            List<ChallengeData> ChallengeData = new List<ChallengeData>();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            int skip = 0, limit = 50, total = 0;
            bool first = true;
            do
            {
                var res = await httpClient.GetAsync($"https://api.dofusdb.fr/challenges?$skip={skip}&$sort[slug.fr]=1&$limit={limit}&categoryId[]=1&categoryId[]=4&iconId[$ne]=0&lang=fr");
                var json = await res.Content.ReadAsStringAsync();
                var parsed = JsonConvert.DeserializeObject<ChallengeApiResponse>(json);
                if (parsed == null) break;
                if (first) { total = parsed.Total; first = false; }
                ChallengeData.AddRange(parsed.Data);
                skip += limit;
            } while (skip < total);

            // distinct on name
            ChallengeData = ChallengeData
                .GroupBy(d => d.Name.Fr)
                .Select(g => g.First())
                .ToList();

            string jsonChallenges = JsonConvert.SerializeObject(ChallengeData, Formatting.Indented);
            File.WriteAllText("challenges.json", jsonChallenges);
        }
    }

    public class ChallengeData
    {
        public int Id { get; set; }
        public int IconId { get; set; }
        public LocalizedText Name { get; set; }
        public LocalizedText Description { get; set; }
    }

    public class ChallengeApiResponse
    {
        public int Total { get; set; }
        public int Limit { get; set; }
        public int Skip { get; set; }
        public List<ChallengeData> Data { get; set; }
    }

    public class DungeonApiResponse
    {
        public int Total { get; set; }
        public int Limit { get; set; }
        public int Skip { get; set; }
        public List<DungeonApiModel> Data { get; set; } = new();
    }

    public class DungeonApiModel
    {
        public int Id { get; set; }
        public int OptimalPlayerLevel { get; set; }
        public List<int> MapIds { get; set; } = new();
        public int EntranceMapId { get; set; }
        public int ExitMapId { get; set; }
        public LocalizedText Name { get; set; }
        public string ClassName { get; set; }
        public int MId { get; set; }
        public LocalizedText Slug { get; set; }
        public List<int> Monsters { get; set; } = new();
        public int Subarea { get; set; }
        public string[] Succes { get; set; } = Array.Empty<string>();
    }

    public class AchievementApiResponse
    {
        public int Total { get; set; }
        public int Limit { get; set; }
        public int Skip { get; set; }
        public List<Achievement> Data { get; set; } = new();
    }

    public class Achievement
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int IconId { get; set; }
        public int Points { get; set; }
        public int Level { get; set; }
        public int Order { get; set; }
        public bool AccountLinked { get; set; }
        public List<int> ObjectiveIds { get; set; } = new();
        public List<int> RewardIds { get; set; } = new();
        public LocalizedText Name { get; set; }
        public LocalizedText Description { get; set; }
        public string ClassName { get; set; }
        public int MId { get; set; }
    }

    public class MonsterApiResponse
    {
        public int Total { get; set; }
        public int Limit { get; set; }
        public int Skip { get; set; }
        public List<Monster> Data { get; set; } = new();
    }

    public class Monster
    {
        public int Id { get; set; }
        public LocalizedText name { get; set; }
        public bool isBoss { get; set; }
        public bool isMiniBoss { get; set; }
        public int gfxId { get; set; } // https://api.dofusdb.fr/img/monsters/{gfxId}.png
    }

    public class LocalizedText
    {
        public string Fr { get; set; }
    }
}