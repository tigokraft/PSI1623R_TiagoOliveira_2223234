using System;
using System.IO;
using System.Text.Json;

namespace login.Helpers
{
    public class ClientSettings
    {
        public string ApiBaseUrl { get; set; } = "http://localhost:5034";

        private static string ConfigFile => "clientsettings.json";

        public static ClientSettings Load()
        {
            if (File.Exists(ConfigFile))
            {
                try
                {
                    string json = File.ReadAllText(ConfigFile);
                    var settings = JsonSerializer.Deserialize<ClientSettings>(json);
                    if (settings != null)
                        return settings;
                }
                catch
                {
                    // ignore malformed config
                }
            }
            return new ClientSettings();
        }

        public void Save()
        {
            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigFile, json);
        }
    }
}
