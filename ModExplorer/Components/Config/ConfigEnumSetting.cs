using System;
using BepInEx.Configuration;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ModExplorer.Components.Config;

public class ConfigEnumSetting(IntPtr cppPtr) : ConfigSetting(cppPtr)
{
    public Il2CppReferenceField<Button> button;
    public Il2CppReferenceField<TextMeshProUGUI> valueText;

    public bool expanded = false;
    private EnumSettingDropdown dropdown;
    private Array enumValues;

    [HideFromIl2Cpp]
    public override void Load(ConfigEntryBase configEntry)
    {
        base.Load(configEntry);
        
        enumValues = Enum.GetValues(ConfigEntry.SettingType);
        dropdown = ModExplorerComponent.Instance.enumSettingDropdown.Value;
        valueText.Value.text = ConfigEntry.BoxedValue.ToString();
        
        button.Value.onClick = new();
        button.Value.onClick.AddListener((UnityAction)(() =>
        {
            if (expanded)
            {
                return;
            }
            
            expanded = true;
            dropdown.CreateItemsFor(enumValues, ConfigEntry.BoxedValue);
            dropdown.onItemSelected += SelectValue;
            dropdown.Show(this);
        }));
    }
    
    [HideFromIl2Cpp]
    private void SelectValue(object value)
    {
        ConfigEntry.BoxedValue = value;
        valueText.Value.text = value.ToString();
    }
}