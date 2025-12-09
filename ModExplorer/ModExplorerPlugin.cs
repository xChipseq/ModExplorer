using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
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
        Assets.Load();
        Harmony.PatchAll();
        IL2CPPChainloader.Instance.Finished += ModDataFinder.Initialize;
    }
}