using HarmonyLib;
using ModExplorer.Components;

namespace ModExplorer.Patches;

[HarmonyPatch(typeof(PassiveButtonManager), nameof(PassiveButtonManager.Update))]
public static class PassiveButtonBlockPatch
{
    private static bool Prefix()
    {
        return ModExplorerComponent.Instance == null;
    }
}