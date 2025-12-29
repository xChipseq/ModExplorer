using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using ModExplorer.Components.Config;
using ModExplorer.Data;
using ModExplorer.Utilities;
using Reactor.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ModExplorer.Components;

public class ModExplorerComponent(IntPtr cppPtr) : MonoBehaviour(cppPtr)
{
    public static ModExplorerComponent Instance { get; private set; }

    public List<ModListElement> elements;
    
    public Il2CppReferenceField<GameObject> mainContainer;
    public Il2CppReferenceField<ScrollRect> scrollRect;
    public Il2CppReferenceField<TMP_InputField> searchInputField;
    public Il2CppReferenceField<RectTransform> modListContainer;

    public Il2CppReferenceField<GameObject> infoTab;
    public Il2CppReferenceField<TextMeshProUGUI> infoMainText;
    public Il2CppReferenceField<TextMeshProUGUI> infoBottomText;
    public Il2CppReferenceField<TextMeshProUGUI> infoLinkText;

    public Il2CppReferenceField<GameObject> configTab;
    public Il2CppReferenceField<RectTransform> configContainer;
    public Il2CppReferenceField<TextMeshProUGUI> configTitleText;
    public Il2CppReferenceField<EnumSettingDropdown> enumSettingDropdown;
        
    public Il2CppReferenceField<Button> closeButton;
    public Il2CppReferenceField<Button> folderButton;
    public Il2CppReferenceField<Button> listButton;
    
    public Il2CppReferenceField<RectTransform> entryDescription;
    public Il2CppReferenceField<Image> entryDescriptionBackground;
    public Il2CppReferenceField<TextMeshProUGUI> entryDescriptionText;
    
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
            listButton.Value.gameObject.SetActive(false);
        }));
        
        folderButton.Value.onClick = new Button.ButtonClickedEvent();
        folderButton.Value.onClick.AddListener((UnityAction)(() =>
        {
            Application.OpenURL("file:///" + Paths.PluginPath.Replace("\\", "/"));
        }));
        
        listButton.Value.onClick = new Button.ButtonClickedEvent();
        listButton.Value.onClick.AddListener((UnityAction)(() =>
        {
            StringBuilder list = new();
            foreach (var mod in ModDataFinder.Mods)
            {
                list.AppendLine($"- {mod.Name} {mod.Version} ({mod.ID})");
            }
            GUIUtility.systemCopyBuffer = list.ToString();
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

            var cfgButton = element.configButton.Value;
            if (data.ConfigFile.Count > 0)
            {
                cfgButton.onClick =  new Button.ButtonClickedEvent();
                cfgButton.onClick.AddListener((UnityAction)(() => OpenConfigPageFor(element)));
            }
            else
            {
                cfgButton.gameObject.SetActive(false);
            }
            
            elements.Add(element);
        }
        
        infoLinkText.Value.GetComponent<LinkEventHandler>().onLinkClicked += Application.OpenURL;

        StartCoroutine(CoScrollToTop(modListContainer.Value).WrapToIl2Cpp());
    }

    public void LateUpdate()
    {
        if (entryDescription.Value.gameObject.activeSelf)
        {
            entryDescriptionBackground.Value.rectTransform.sizeDelta =
                entryDescriptionText.Value.rectTransform.sizeDelta + new Vector2(50, 50); // size + padding
            entryDescription.Value.position = Input.mousePosition;
        }
    }
    
    public void ShowDescriptionBox(string description)
    {
        entryDescriptionText.Value.text = description.WrapLines(40);
        entryDescriptionBackground.Value.rectTransform.sizeDelta =
            entryDescriptionText.Value.rectTransform.sizeDelta + new Vector2(50, 50); // size + padding
        entryDescription.Value.position = Input.mousePosition;
        entryDescription.Value.gameObject.SetActive(true);
    }

    public void HideDescriptionBox()
    {
        entryDescription.Value.gameObject.SetActive(false);
    }

    [HideFromIl2Cpp]
    private IEnumerator CoScrollToTop(RectTransform container)
    {
        yield return new WaitForEndOfFrame();
        
        // The only thing that worked
        container.localPosition = new Vector3(container.localPosition.x, -9999, 0);
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
        {
            var dependencies = data.Dependencies.Select(ModDataFinder.GetModData).Select(x => x.Name);
            bottomBuilder.AppendLine($"Dependencies: {string.Join(", ", dependencies)}");
        }
        infoBottomText.Value.text = bottomBuilder.ToString();
        
        StringBuilder linkBuilder = new();
        foreach (var (link, url) in data.Links)
        {
            linkBuilder.AppendLine($"<link={url}>{link}</link>");
        }
        infoLinkText.Value.text = linkBuilder.ToString();
        
        infoTab.Value.SetActive(true);
        configTab.Value.SetActive(false);
    }

    public void OpenConfigPageFor(ModListElement element)
    {
        configTitleText.Value.text = $"{element.ModData.Name} <size=55%><color=grey>config</color></size>";

        var categories = new Dictionary<string, Dictionary<ConfigEntryBase, ConfigSetting>>();
        foreach (var (definition, entry) in element.ModData.ConfigFile)
        {
            if (ModExplorerPlugin.ModConfig.ExcludeMiraOptions.Value && MiraCompatibility.IsMiraOption(element.ModData.ID, definition))
            {
                continue;
            }
            if (MiraCompatibility.IsMiraRoleOption(element.ModData.ID, definition) && ModExplorerPlugin.ModConfig.ExcludeMiraRoleOptions.Value)
            {
                continue;
            }
            
            if (!categories.TryGetValue(definition.Section, out var list))
            {
                categories.Add(definition.Section, new());
                list = categories[definition.Section];
            }
            
            ConfigSetting setting = null;
            switch (entry.BoxedValue)
            {
                case bool:
                    setting = Instantiate(Assets.BoolSettingPrefab.Get()).GetComponent<ConfigBoolSetting>();
                    break;
                case int or byte:
                    setting = Instantiate(Assets.IntSettingPrefab.Get()).GetComponent<ConfigIntSetting>();
                    break;
                case float or double:
                    setting = Instantiate(Assets.FloatSettingPrefab.Get()).GetComponent<ConfigFloatSetting>();
                    break;
                case string:
                    setting = Instantiate(Assets.StringSettingPrefab.Get()).GetComponent<ConfigStringSetting>();
                    break;
                default:
                    if (entry.SettingType.IsEnum)
                    {
                        setting = Instantiate(Assets.EnumSettingPrefab.Get()).GetComponent<ConfigEnumSetting>();
                    }
                    break;
            }

            if (setting == null)
            {
                continue;
            }

            setting.name = definition.ToString();
            list.Add(entry, setting);
        }
        
        configContainer.Value.DestroyChildren();
        foreach (var (category, settings) in categories)
        {
            var categoryLabel = Instantiate(Assets.SettingCategoryPrefab.Get(), configContainer).GetComponent<ConfigCategoryLabel>();
            categoryLabel.Load(category);
            
            foreach (var (entry, setting) in settings)
            {
                setting.transform.SetParent(configContainer);
                setting.Load(entry);
            }
        }
        
        
        StartCoroutine(CoScrollToTop(configContainer.Value).WrapToIl2Cpp());

        infoTab.Value.SetActive(false);
        configTab.Value.SetActive(true);
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