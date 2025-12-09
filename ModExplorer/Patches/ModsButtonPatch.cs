using HarmonyLib;
using ModExplorer.Components;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace ModExplorer.Patches;

[HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
public static class Patch
{
    private static void Postfix(MainMenuManager __instance)
    {
        var button = Object.Instantiate(__instance.quitButton).GetComponent<PassiveButton>();
        var aspect = button.GetComponent<AspectPosition>();
        button.GetComponent<SceneChanger>().Destroy();
        button.GetComponent<ConditionalHide>().Destroy();
        
        button.gameObject.name = "ModsButton";
        button.OnClick = new();
        button.OnClick.AddListener((UnityAction)(() =>
        {
            ModExplorerComponent.Create();
            button.ReceiveMouseOut();
        }));
        
        aspect.DistanceFromEdge = new Vector3(1f, 1.6f, 0);
        aspect.Alignment = AspectPosition.EdgeAlignments.Right;
        aspect.AdjustPosition();

        var text = button.transform.Find("FontPlacer").GetComponentInChildren<TextMeshPro>();
        text.GetComponent<TextTranslatorTMP>().Destroy();
        text.text = "MODS";
    }
}