using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using ModExplorer.Components;
using ModExplorer.Data;
using Reactor;

namespace ModExplorer;

[BepInAutoPlugin("chipseq.modexplorer")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
public partial class ModExplorerPlugin : BasePlugin
{
    public Harmony Harmony { get; } = new(Id);
    
    public override void Load()
    {
        ClassInjector.RegisterTypeInIl2Cpp<ModExplorerComponent>();
        ClassInjector.RegisterTypeInIl2Cpp<ModListElement>();
        ClassInjector.RegisterTypeInIl2Cpp<LinkEventHandler>();
        Harmony.PatchAll();
        
        Assets.Load();
        IL2CPPChainloader.Instance.Finished += ModDataFinder.Initialize;
    }
}