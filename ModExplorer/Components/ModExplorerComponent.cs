using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using ModExplorer.Data;
using Reactor.Utilities;
using Reactor.Utilities.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ModExplorer.Components;

[RegisterInIl2Cpp]
public class ModExplorerComponent(IntPtr cppPtr) : MonoBehaviour(cppPtr)
{
    public static ModExplorerComponent Instance { get; private set; }

    public List<ModListElement> elements;
    public Il2CppReferenceField<GameObject> mainContainer;
    public Il2CppReferenceField<ScrollRect> scrollRect;
    public Il2CppReferenceField<TMP_InputField> searchInputField;
    public Il2CppReferenceField<RectTransform> modListContainer;
    public Il2CppReferenceField<TextMeshProUGUI> infoMainText;
    public Il2CppReferenceField<TextMeshProUGUI> infoBottomText;
    public Il2CppReferenceField<TextMeshProUGUI> infoLinkText;
    public Il2CppReferenceField<Button> closeButton;
    public Il2CppReferenceField<Button> folderButton;
    
    public static ModExplorerComponent Create()
    {
        var explorer = Instantiate(Assets.MenuPrefab.Get()).GetComponent<ModExplorerComponent>();
        return explorer;
    }

    public void Awake()
    {
        if (Instance)
        {
            Logger<ModExplorerPlugin>.Warning($"An instance of the explorer already exists. Destroying the new one.");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        var transition = mainContainer.Value.AddComponent<TransitionOpen>();
        transition.OnClose.AddListener((UnityAction)(() => Destroy(gameObject)));
        
        closeButton.Value.onClick = new Button.ButtonClickedEvent();
        closeButton.Value.onClick.AddListener((UnityAction)(() =>
        {
            transition.Close();
            closeButton.Value.gameObject.SetActive(false);
            folderButton.Value.gameObject.SetActive(false);
        }));
        
        folderButton.Value.onClick = new Button.ButtonClickedEvent();
        folderButton.Value.onClick.AddListener((UnityAction)(() =>
        {
            Application.OpenURL("file:///" + Paths.PluginPath.Replace("\\", "/"));
        }));

        searchInputField.Value.onValueChanged = new TMP_InputField.OnChangeEvent();
        searchInputField.Value.onValueChanged.AddListener((UnityAction<string>)SetSearchResults);

        infoMainText.Value.text = string.Empty;
        infoBottomText.Value.text = string.Empty;
        infoLinkText.Value.text = string.Empty;
        
        modListContainer.Value.DestroyChildren();
        elements = new();
        foreach (var data in ModDataFinder.Mods)
        {
            var element = ModListElement.Create(modListContainer);
            var btn = element.GetComponent<Button>();
            element.SetFromData(data);
            btn.onClick = new Button.ButtonClickedEvent();
            btn.onClick.AddListener((UnityAction)(() => OpenInfoPageFor(element)));
            elements.Add(element);
        }
    }

    public void OpenInfoPageFor(ModListElement element)
    {
        // do are you have stupid
        var colors = element.button.Value.colors;
        elements.Do(x =>
        {
            x.button.Value.colors = colors with { normalColor = Color.white };
        });
        element.button.Value.colors = colors with { normalColor = Palette.AcceptedGreen };
        
        var data = element.ModData;
        StringBuilder mainBuilder = new();
        mainBuilder.AppendLine(
            $"<size=150>{data.Name}</size> <color=grey><font=\"LiberationSans SDF\">v{data.Version}</font></color>");
        if (data.Authors.Length > 0)
            mainBuilder.AppendLine($"<font=\"LiberationSans SDF\">by {string.Join(", ", data.Authors)}</font>\n");
        if (!data.Description.IsNullOrWhiteSpace())
            mainBuilder.AppendLine($"<font=\"LiberationSans SDF\">{data.Description}</font>");
        infoMainText.Value.text = mainBuilder.ToString();
        
        StringBuilder bottomBuilder = new();
        bottomBuilder.AppendLine($"Mod ID: {data.ID}");
        if (!data.License.IsNullOrWhiteSpace())
            bottomBuilder.AppendLine($"License: {data.License}");
        if (data.Dependencies.Length > 0)
            bottomBuilder.AppendLine($"Dependencies: {string.Join(", ", data.Dependencies)}");
        infoBottomText.Value.text = bottomBuilder.ToString();
        
        StringBuilder linkBuilder = new();
        foreach (var (type, link) in data.Links)
        {
            linkBuilder.AppendLine($"{type}");
        }
        infoLinkText.Value.text = linkBuilder.ToString();
    }

    public void SetSearchResults(string search)
    {
        if (string.IsNullOrEmpty(search))
        {
            elements.Do(x => x.gameObject.SetActive(true));
            return;
        }
        
        var valid = elements
            .Where(x => x.ModData.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase) ||
                        x.ModData.Tags.Any(t => t.Contains(search, StringComparison.InvariantCultureIgnoreCase)))
            .OrderBy(x => x.ModData.Name)
            .ThenBy(x => x.ModData.Name.Equals(search, StringComparison.OrdinalIgnoreCase))
            .ThenBy(x => x.ModData.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
        
        elements.Do(x => x.gameObject.SetActive(false));
        valid.Do(x => x.gameObject.SetActive(true));
    }
}