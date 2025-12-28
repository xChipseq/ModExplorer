using BepInEx.Configuration;

namespace ModExplorer;

public class ModExplorerConfig
{
    public ConfigEntry<bool> PrettyCategories { get; init; }
    public ConfigEntry<bool> PrettySettings { get; init; }
    
    public ConfigEntry<int> IntSetting { get; init; }
    public ConfigEntry<float> FloatSetting { get; init; }
    
    public ModExplorerConfig(ConfigFile config)
    {
        PrettyCategories = config.Bind("Mod Configs", "Pretty Categories", true,
            "Makes config categories visually better by adding spaces when there's lack of them.");
        PrettySettings = config.Bind("Mod Configs", "Pretty Settings", true,
            "Makes config entries visually better by adding spaces when there's lack of them.");

        IntSetting = config.Bind("Example", "Example Int", 0);
        FloatSetting = config.Bind("Example", "Example Int", 4.2f);
    }
}