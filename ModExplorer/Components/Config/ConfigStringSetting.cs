using System;
using BepInEx.Configuration;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using TMPro;
using UnityEngine.Events;

namespace ModExplorer.Components.Config;

public class ConfigStringSetting(IntPtr cppPtr) : ConfigSetting(cppPtr)
{
    public Il2CppReferenceField<TMP_InputField> textInput;
    
    [HideFromIl2Cpp]
    public override void Load(ConfigEntryBase configEntry)
    {
        base.Load(configEntry);
        UpdateValue();
        
        textInput.Value.onSubmit = new TMP_InputField.SubmitEvent();
        textInput.Value.onSubmit.AddListener((UnityAction<string>)(x =>
        {
            configEntry.BoxedValue = x;
            UpdateValue();
        }));
    }

    private void UpdateValue()
    {
        textInput.Value.text = ConfigEntry.BoxedValue.ToString();
    }
}