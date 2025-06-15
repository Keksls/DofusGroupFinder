using DofusGroupFinder.Domain.Entities;
using DofusGroupFinder.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http.Headers;

// DB config
var connectionString = "Host=localhost;Port=5432;Database=DofusGroupFinderDb;Username=postgres;Password=DofusGroup123!";
var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
optionsBuilder.UseNpgsql(connectionString);
using var dbContext = new ApplicationDbContext(optionsBuilder.Options);

// HttpClient config
var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

// Download all dungeons (pagination)
int limit = 25;
int skip = 0;
int total = 0;
bool firstRequest = true;

List<DungeonApiModel> allDungeons = new();

do
{
    var url = $"https://api.dofusdb.fr/dungeons?$skip={skip}&$limit={limit}&lang=fr";
    Console.WriteLine($"Fetching: skip={skip}");

    var response = await httpClient.GetAsync(url);
    if (!response.IsSuccessStatusCode)
    {
        Console.WriteLine($"Error fetching data: {response.StatusCode}");
        break;
    }

    var json = await response.Content.ReadAsStringAsync();
    var apiResponse = JsonConvert.DeserializeObject<DungeonApiResponse>(json);

    if (apiResponse == null || apiResponse.Data == null)
    {
        Console.WriteLine("Empty response");
        break;
    }

    if (firstRequest)
    {
        total = apiResponse.Total;
        firstRequest = false;
    }

    allDungeons.AddRange(apiResponse.Data);
    skip += limit;

} while (skip < total);

Console.WriteLine($"Total dungeons fetched: {allDungeons.Count}");

// Export local JSON
var exportJson = JsonConvert.SerializeObject(allDungeons, Formatting.Indented);
File.WriteAllText("dungeons_export.json", exportJson);
Console.WriteLine("Exported to dungeons_export.json ✅");

// Injection en base (après export)
foreach (var dungeonApi in allDungeons)
{
    var existingDungeon = await dbContext.Dungeons.FirstOrDefaultAsync(d => d.ExternalId == dungeonApi.Id);

    if (existingDungeon == null)
    {
        dbContext.Dungeons.Add(new Dungeon
        {
            Id = Guid.NewGuid(),
            ExternalId = dungeonApi.Id,
            Name = dungeonApi.Name.Fr,
            MinLevel = dungeonApi.OptimalPlayerLevel,
            MaxLevel = dungeonApi.OptimalPlayerLevel
        });
    }
    else
    {
        existingDungeon.Name = dungeonApi.Name.Fr;
        existingDungeon.MinLevel = dungeonApi.OptimalPlayerLevel;
        existingDungeon.MaxLevel = dungeonApi.OptimalPlayerLevel;
    }
}

await dbContext.SaveChangesAsync();

Console.WriteLine("Bake terminé ✅");

// ---- Models pour désérialiser l'API ----

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
    public DungeonName Name { get; set; } = null!;
}

public class DungeonName
{
    public string Fr { get; set; } = null!;
}
