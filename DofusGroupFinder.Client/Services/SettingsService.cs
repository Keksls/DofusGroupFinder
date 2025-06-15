using DofusGroupFinder.Client.Models;
using System.IO;
using System.Text.Json;

namespace DofusGroupFinder.Client.Services
{
    public class SettingsService
    {
        private const string FilePath = "Config/settings.json";

        public void SaveServer(string server)
        {
            Directory.CreateDirectory("Config");
            File.WriteAllText(FilePath, JsonSerializer.Serialize(new SettingsObject { Server = server }));
        }

        public string LoadServer()
        {
            if (!File.Exists(FilePath))
                return ServerList.Servers[0];

            var json = File.ReadAllText(FilePath);
            var obj = JsonSerializer.Deserialize<SettingsObject>(json);
            return obj?.Server ?? string.Empty;
        }

        private class SettingsObject
        {
            public string Server { get; set; } = string.Empty;
        }
    }
}