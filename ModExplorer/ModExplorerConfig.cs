using BepInEx.Configuration;

namespace ModExplorer;

public class ModExplorerConfig
{
    public ConfigEntry<bool> HideReactorCredits { get; init; }
    public ConfigEntry<bool> PrettyCategories { get; init; }
    public ConfigEntry<bool> PrettySettings { get; init; }
    public ConfigEntry<bool> ExcludeMiraOptions { get; init; }
    public ConfigEntry<bool> ExcludeMiraRoleOptions { get; init; }
    
    public ConfigEntry<int> IntSetting { get; init; }
    public ConfigEntry<float> FloatSetting { get; init; }
    public ConfigEntry<byte> ByteSetting { get; init; }
    
    public ModExplorerConfig(ConfigFile config)
    {
        HideReactorCredits = config.Bind("General", "Hide Reactor Credits", true,
            "Hides ReactorCredits in the main menu when you don't need two separate lists of mods");
        PrettyCategories = config.Bind("Mod Configs", "Pretty Categories", true,
            "Makes config categories visually better by adding spaces when there's lack of them");
        PrettySettings = config.Bind("Mod Configs", "Pretty Settings", true,
            "Makes config entries visually better by adding spaces when there's lack of them");
        ExcludeMiraOptions = config.Bind("Mod Configs", "Exclude MiraAPI Options", true,
            "Excludes custom game options in configs from plugins that use Mira API");
        ExcludeMiraRoleOptions = config.Bind("Mod Configs", "Exclude MiraAPI Role Options", true,
            "Excludes custom role chance and count options in configs from plugins that use Mira API");
        
        IntSetting = config.Bind("Example", "Example Int", 0);
        FloatSetting = config.Bind("Example", "Example Float", 4.2f);
        ByteSetting = config.Bind("Example", "Example Byte", (byte)8);
    }
}