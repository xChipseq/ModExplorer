using System;
using BepInEx.Configuration;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ModExplorer.Components.Config;

public class ConfigIntSetting(IntPtr cppPtr) : ConfigSetting(cppPtr)
{
    public Il2CppReferenceField<TMP_InputField> intInput;
    public Il2CppReferenceField<Button> plusButton;
    public Il2CppReferenceField<Button> minusButton;
    
    [HideFromIl2Cpp]
    public override void Load(ConfigEntryBase configEntry)
    {
        base.Load(configEntry);
        UpdateValue();
        
        intInput.Value.onSubmit = new TMP_InputField.SubmitEvent();
        intInput.Value.onSubmit.AddListener((UnityAction<string>)(x =>
        {
            SetValue(x);
            UpdateValue();
        }));
        
        plusButton.Value.onClick = new Button.ButtonClickedEvent();
        plusButton.Value.onClick.AddListener((UnityAction)(() =>
        {
            ChangeValue(1);
            UpdateValue();
        }));
        
        minusButton.Value.onClick = new Button.ButtonClickedEvent();
        minusButton.Value.onClick.AddListener((UnityAction)(() =>
        {
            ChangeValue(-1);
            UpdateValue();
        }));
    }

    private void SetValue(object value)
    {
        ConfigEntry.SetSerializedValue(value.ToString());
    }
    
    private void ChangeValue(object change)
    {
        if (ConfigEntry.BoxedValue is byte)
        {
            SetValue((byte)ConfigEntry.BoxedValue + (int)change);
        }
        else
        {
            SetValue((int)ConfigEntry.BoxedValue + (int)change);
        }
    }

    private void UpdateValue()
    {
        intInput.Value.text = ConfigEntry.BoxedValue.ToString();
    }
}