using Newtonsoft.Json;

namespace dsbot;

public class Settings
{
    public string Token { get; set; } = string.Empty;
    public string Prefix { get; set; } = string.Empty;

    public async Task ReadSettings()
    {
        using StreamReader streamReader = new("config.json");

        var json = await streamReader.ReadToEndAsync()
            ?? throw new Exception("json is null");

        SettingsStucture settingsStucture = JsonConvert.DeserializeObject<SettingsStucture>(json);

        this.Token = settingsStucture.Token;
        this.Prefix = settingsStucture.Prefix;
    }

    internal sealed class SettingsStucture
    {
        public string Token { get; set; } = string.Empty;
        public string Prefix { get; set; } = string.Empty;    
    }
}