using BepInEx.Configuration;

namespace ModExplorer;

public class ModExplorerConfig
{
    public ConfigEntry<bool> HideReactorCredits { get; }
    public ConfigEntry<bool> PrettyCategories { get; }
    public ConfigEntry<bool> PrettySettings { get; }
    public ConfigEntry<bool> ExcludeMiraOptions { get; }
    public ConfigEntry<bool> ExcludeMiraRoleOptions { get; }
    
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
    }
}