//using Zaapix.Domain.Entities;
//using Zaapix.Infrastructure.Persistence;
//using Microsoft.EntityFrameworkCore;
//using Newtonsoft.Json;
//using System.Net.Http.Headers;
//using System.Text.RegularExpressions;

using Zaapix.Baker;

SuccesBaker baker = new();
await baker.Bake();

//// DB config
//var connectionString = "Host=localhost;Port=5432;Database=ZaapixDb;Username=postgres;Password=DofusGroup123!";
//var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
//optionsBuilder.UseNpgsql(connectionString);
//using var dbContext = new ApplicationDbContext(optionsBuilder.Options);

//// HttpClient config
//var httpClient = new HttpClient();
//httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

//// Download all dungeons (pagination)
//int limit = 25;
//int skip = 0;
//int total = 0;
//bool firstRequest = true;

//List<DungeonApiModel> allDungeons = new();

//do
//{
//    var url = $"https://api.dofusdb.fr/dungeons?$skip={skip}&$limit={limit}&lang=fr";
//    Console.WriteLine($"Fetching: skip={skip}");

//    var response = await httpClient.GetAsync(url);
//    if (!response.IsSuccessStatusCode)
//    {
//        Console.WriteLine($"Error fetching data: {response.StatusCode}");
//        break;
//    }

//    var json = await response.Content.ReadAsStringAsync();
//    var apiResponse = JsonConvert.DeserializeObject<DungeonApiResponse>(json);

//    if (apiResponse == null || apiResponse.Data == null)
//    {
//        Console.WriteLine("Empty response");
//        break;
//    }

//    if (firstRequest)
//    {
//        total = apiResponse.Total;
//        firstRequest = false;
//    }

//    allDungeons.AddRange(apiResponse.Data);
//    skip += limit;

//} while (skip < total);

//Console.WriteLine($"Total dungeons fetched: {allDungeons.Count}");
//int successLimit = 50;
//int successSkip = 0;
//int successTotal = 0;
//bool firstSuccessRequest = true;

//List<Success> allSuccesses = new();
//do
//{
//    var url = $"https://api.dofusdb.fr/achievements?$skip={successSkip}&$limit={successLimit}&categoryId=12&$populate=false&lang=fr";
//    Console.WriteLine($"Fetching successes: skip={successSkip}");

//    var response = await httpClient.GetAsync(url);
//    if (!response.IsSuccessStatusCode)
//    {
//        Console.WriteLine($"Error fetching successes: {response.StatusCode}");
//        break;
//    }

//    var json = await response.Content.ReadAsStringAsync();
//    var apiResponse = JsonConvert.DeserializeObject<SuccessApiResponse>(json);

//    if (apiResponse?.Data == null)
//    {
//        Console.WriteLine("Empty success response");
//        break;
//    }

//    if (firstSuccessRequest)
//    {
//        successTotal = apiResponse.Total;
//        firstSuccessRequest = false;
//    }

//    allSuccesses.AddRange(apiResponse.Data);
//    successSkip += successLimit;

//} while (successSkip < successTotal);

//Console.WriteLine($"Total dungeon-related successes fetched: {allSuccesses.Count}");

//// Extrait les types de succès : "Duo", "Hardi", etc.
//List<string> ExtractSuccessTypesForDungeon(int dungeonId, List<Success> allSuccesses)
//{
//    return allSuccesses
//        .Where(s => s.m_id == dungeonId)
//        .Select(s =>
//        {
//            var name = s.name?.Fr ?? "";
//            var match = Regex.Match(name, @"\(([^)]+)\)");
//            return match.Success ? match.Groups[1].Value : null;
//        })
//        .Where(type => !string.IsNullOrEmpty(type))
//        .Distinct()
//        .ToList();
//}


//// distinct on name
//allDungeons = allDungeons
//    .GroupBy(d => d.Name.Fr)
//    .Select(g => g.First())
//    .ToList();

//// Export local JSON
//var exportJson = JsonConvert.SerializeObject(allDungeons, Formatting.Indented);
//File.WriteAllText("dungeons_export.json", exportJson);
//Console.WriteLine("Exported to dungeons_export.json ✅");

//// clear existing dungeons in the database
//await dbContext.Dungeons.ExecuteDeleteAsync();

//// Injection en base (après export)
//foreach (var dungeonApi in allDungeons)
//{
//    var successTypes = ExtractSuccessTypesForDungeon(dungeonApi.Id, allSuccesses);

//    var existingDungeon = await dbContext.Dungeons.FirstOrDefaultAsync(d => d.ExternalId == dungeonApi.Id);

//    if (existingDungeon == null)
//    {
//        dbContext.Dungeons.Add(new Dungeon
//        {
//            Id = Guid.NewGuid(),
//            ExternalId = dungeonApi.Id,
//            Name = dungeonApi.Name.Fr,
//            MinLevel = dungeonApi.OptimalPlayerLevel,
//            MaxLevel = dungeonApi.OptimalPlayerLevel,
//            Succes = successTypes.ToArray()
//        });
//    }
//    else
//    {
//        existingDungeon.Name = dungeonApi.Name.Fr;
//        existingDungeon.MinLevel = dungeonApi.OptimalPlayerLevel;
//        existingDungeon.MaxLevel = dungeonApi.OptimalPlayerLevel;
//        existingDungeon.Succes = successTypes.ToArray();
//    }
//}

//await dbContext.SaveChangesAsync();

//Console.WriteLine("Bake terminé ✅");

//// ---- Models pour désérialiser l'API ----

//public class DungeonApiResponse
//{
//    public int Total { get; set; }
//    public int Limit { get; set; }
//    public int Skip { get; set; }
//    public List<DungeonApiModel> Data { get; set; } = new();
//}

//public class DungeonApiModel
//{
//    public int Id { get; set; }
//    public int OptimalPlayerLevel { get; set; }
//    public Name Name { get; set; } = null!;
//}

//public class Name
//{
//    public string Fr { get; set; }
//}

//public class Success
//{
//    public int id { get; set; }
//    public int m_id { get; set; }
//    public Name name { get; set; }
//}

//public class SuccessApiResponse
//{
//    public int Total { get; set; }
//    public int Limit { get; set; }
//    public int Skip { get; set; }
//    public List<Success> Data { get; set; }
//}
