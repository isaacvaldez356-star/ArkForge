using System.Text.Json;
using ArkForge.Common.Models;

namespace ArkForge.Configuration
{
    public class ConfigService
    {
        private readonly string _configPath;

        public ConfigService()
        {
            var folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "ArkForge");

            Directory.CreateDirectory(folder);
            _configPath = Path.Combine(folder, "config.json");
        }

        public ServerConfig Load()
        {
            if (!File.Exists(_configPath))
            {
                var defaultConfig = new ServerConfig();
                Save(defaultConfig);
                return defaultConfig;
            }

            var json = File.ReadAllText(_configPath);
            return JsonSerializer.Deserialize<ServerConfig>(json) ?? new ServerConfig();
        }

        public void Save(ServerConfig config)
        {
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_configPath, json);
        }
    }
}