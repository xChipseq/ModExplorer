using System;
using BepInEx.Configuration;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using ModExplorer.Utilities;
using TMPro;
using UnityEngine;

namespace ModExplorer.Components.Config;

public abstract class ConfigSetting(IntPtr cppPtr) : MonoBehaviour(cppPtr), IConfigElement
{
    public Il2CppReferenceField<TextMeshProUGUI> text;
    
    protected ConfigEntryBase ConfigEntry;

    [HideFromIl2Cpp]
    public virtual void Load(ConfigEntryBase configEntry)
    {
        ConfigEntry = configEntry;
        
        string entry = ModExplorerPlugin.ModConfig.PrettySettings.Value
            ? Utils.SpaceOutString(ConfigEntry.Definition.Key)
            : ConfigEntry.Definition.Key;
        text.Value.text = $"<link=desc>{entry}</link>";

        if (configEntry.Description.Description.IsNullOrWhiteSpace())
        {
            return;
        }
        
        var handler = text.Value.gameObject.AddComponent<LinkEventHandler>();
        handler.onLinkClicked += _ =>
        {
            ModExplorerComponent.Instance.ShowDescriptionBox(configEntry.Description.Description);
        };
        
        handler.onLinkFocusLost += _ =>
        {
            ModExplorerComponent.Instance.HideDescriptionBox();
        };
    }
}