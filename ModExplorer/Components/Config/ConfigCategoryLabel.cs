using System;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using ModExplorer.Utilities;
using TMPro;
using UnityEngine;

namespace ModExplorer.Components.Config;

public class ConfigCategoryLabel(IntPtr cppPtr) : MonoBehaviour(cppPtr), IConfigElement
{
    public Il2CppReferenceField<TextMeshProUGUI> titleText;

    [HideFromIl2Cpp]
    public void Load(string category)
    {
        titleText.Value.text = ModExplorerPlugin.ModConfig.PrettyCategories.Value
            ? Utils.SpaceOutString(category)
            : category;
    }
}