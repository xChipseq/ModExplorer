using System;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using ModExplorer.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModExplorer.Components;

public class ModListElement(IntPtr cppPtr) : MonoBehaviour(cppPtr)
{
    public ModData ModData;

    public Il2CppReferenceField<Button> button;
    public Il2CppReferenceField<TextMeshProUGUI> text;
    public Il2CppReferenceField<Image> icon;
    public Il2CppReferenceField<Button> configButton;

    public static ModListElement Create(Transform parent)
    {
        var element = Instantiate(Assets.ModListElementPrefab.Get(), parent);
        return element.GetComponent<ModListElement>();
    }
    
    [HideFromIl2Cpp]
    public void SetFromData(ModData data)
    {
        ModData = data;
        name = data.ID;
        text.Value.text =
            $"{data.Name}\n<color=grey><font=\"LiberationSans SDF\"><size=24>v{data.Version}</size></font></color>";
        if (data.Icon)
        {
            icon.Value.sprite = data.Icon;
        }
        else
        {
            icon.Value.gameObject.SetActive(false);
            text.Value.transform.localPosition -= new Vector3(107, 0, 0);
        }
    }
}
