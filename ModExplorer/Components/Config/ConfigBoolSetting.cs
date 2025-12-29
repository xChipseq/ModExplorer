using System;
using BepInEx.Configuration;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ModExplorer.Components.Config;

public class ConfigBoolSetting(IntPtr cppPtr) : ConfigSetting(cppPtr)
{
    public Il2CppReferenceField<Button> toggleButton;
    public Il2CppReferenceField<Image> checkmark;

    [HideFromIl2Cpp]
    public override void Load(ConfigEntryBase configEntry)
    {
        base.Load(configEntry);
        UpdateValue();
        
        toggleButton.Value.onClick = new Button.ButtonClickedEvent();
        toggleButton.Value.onClick.AddListener((UnityAction)(() =>
        {
            ConfigEntry.BoxedValue = !(bool)ConfigEntry.BoxedValue;
            UpdateValue();
        }));
    }

    private void UpdateValue()
    {
        checkmark.Value.gameObject.SetActive((bool)ConfigEntry.BoxedValue);
    }
}