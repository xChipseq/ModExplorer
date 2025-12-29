using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using ModExplorer.Components;
using ModExplorer.Components.Config;
using ModExplorer.Data;
using Reactor;

namespace ModExplorer;

[BepInAutoPlugin("chipseq.modexplorer")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
public partial class ModExplorerPlugin : BasePlugin
{
    internal static Harmony Harmony { get; } = new(Id);

    public static ModExplorerConfig ModConfig { get; private set; }
    
    public override void Load()
    {
        Assets.Load();
        ModConfig = new(Config);
        
        ClassInjector.RegisterTypeInIl2Cpp<ConfigEnumSetting>();
        ClassInjector.RegisterTypeInIl2Cpp<EnumSettingDropdown>();
        ClassInjector.RegisterTypeInIl2Cpp<ModExplorerComponent>();
        ClassInjector.RegisterTypeInIl2Cpp<ModListElement>();
        ClassInjector.RegisterTypeInIl2Cpp<LinkEventHandler>();
        ClassInjector.RegisterTypeInIl2Cpp<ConfigSetting>();
        ClassInjector.RegisterTypeInIl2Cpp<ConfigCategoryLabel>();
        ClassInjector.RegisterTypeInIl2Cpp<ConfigBoolSetting>();
        ClassInjector.RegisterTypeInIl2Cpp<ConfigIntSetting>();
        ClassInjector.RegisterTypeInIl2Cpp<ConfigFloatSetting>();
        ClassInjector.RegisterTypeInIl2Cpp<ConfigStringSetting>();
        
        Harmony.PatchAll();
        IL2CPPChainloader.Instance.Finished += ModDataFinder.Initialize;
    }
}