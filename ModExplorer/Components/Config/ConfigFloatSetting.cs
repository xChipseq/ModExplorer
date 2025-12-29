using System;
using BepInEx.Configuration;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using TMPro;
using UnityEngine.Events;

namespace ModExplorer.Components.Config;

public class ConfigFloatSetting(IntPtr cppPtr) : ConfigSetting(cppPtr)
{
    public Il2CppReferenceField<TMP_InputField> floatInput;
    
    [HideFromIl2Cpp]
    public override void Load(ConfigEntryBase configEntry)
    {
        base.Load(configEntry);
        UpdateValue();
        
        floatInput.Value.onSubmit = new TMP_InputField.SubmitEvent();
        floatInput.Value.onSubmit.AddListener((UnityAction<string>)(x =>
        {
            configEntry.SetSerializedValue(x);
            UpdateValue();
        }));
    }

    private void UpdateValue()
    {
        floatInput.Value.text = ConfigEntry.BoxedValue.ToString();
    }
}