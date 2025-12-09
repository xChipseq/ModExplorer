using HarmonyLib;
using ModExplorer.Components;
using Rewired;

namespace ModExplorer.Patches;

[HarmonyPatch(typeof(InputManager_Base), nameof(InputManager_Base.Update))]
public static class InputBlockPatch
{
    private static bool Prefix()
    {
        return ModExplorerComponent.Instance == null;
    }
}