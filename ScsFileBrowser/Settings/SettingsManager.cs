using Newtonsoft.Json;
using ScsFileBrowser.Logging;

namespace ScsFileBrowser.Settings;

internal class SettingsManager
{
    private static readonly string _settingsFilePath =
        Path.Combine(Consts.AppDirPath,
            "config.cfg");

    private static readonly Lazy<SettingsManager> _settingsManager = new(() => new SettingsManager());

    internal SettingsManager()
    {
        Load();
    }

    internal static SettingsManager Instance => _settingsManager.Value;

    internal Settings Settings { get; set; } = new();

    internal void Load()
    {
        if (!File.Exists(_settingsFilePath))
        {
            Settings = new Settings();
            return;
        }

        Logger.Instance.Debug($"Loading settings from '{_settingsFilePath}'");
        var settingsContent = File.ReadAllText(_settingsFilePath);
        var settings = JsonConvert.DeserializeObject<Settings>(settingsContent);
        if (settings != null)
            Settings = settings;
        else
        {
            Logger.Instance.Error("Found invalid settings, loading defaults...");
            Settings = new Settings();
        }
    }


    internal void Save()
    {
        if (!Directory.Exists(Path.GetDirectoryName(_settingsFilePath)))
        {
            var dirName = Path.GetDirectoryName(_settingsFilePath);
            if (dirName == null)
            {
                Logger.Instance.Error(
                    $"Could not save settings, could not get directory name for '{_settingsFilePath}'");
                return;
            }

            Directory.CreateDirectory(dirName);
        }

        Logger.Instance.Debug($"Saving settings to '{_settingsFilePath}'");
        File.WriteAllText(_settingsFilePath, JsonConvert.SerializeObject(Settings, Formatting.Indented));
    }
}
