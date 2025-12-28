using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Reactor;
using Reactor.Patches;
using Reactor.Utilities.Extensions;
using Version = SemanticVersioning.Version;

namespace ModExplorer.Patches;

[HarmonyPatch(typeof(ReactorVersionShower), nameof(ReactorVersionShower.UpdateText))]
public class ReactorShowerPatch
{
    public static void Postfix()
    {
        if (!ModExplorerPlugin.ModConfig.HideReactorCredits.Value) return;
        if (ReactorVersionShower.Text == null) return;
        ReactorVersionShower.Text.text = "Reactor " + Version.Parse(ReactorPlugin.Version).WithoutBuild();
        ReactorVersionShower.Text.text += "\nBepInEx " + Paths.BepInExVersion.WithoutBuild();
        ReactorVersionShower.Text.text += "\nMods: " + IL2CPPChainloader.Instance.Plugins.Count;
    }
}