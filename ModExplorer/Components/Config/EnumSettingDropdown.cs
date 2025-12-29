using System;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using Il2CppSystem.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ModExplorer.Components.Config;

public class EnumSettingDropdown(IntPtr cppPtr) : MonoBehaviour(cppPtr)
{
    public Il2CppReferenceField<RectTransform> itemRect;
    public Il2CppReferenceField<Toggle> templateItem;

    [HideFromIl2Cpp]
    public event Action<object> onItemSelected;
    private ConfigEnumSetting current;
    private List<Toggle> itemsObjects;
    
    [HideFromIl2Cpp]
    public void CreateItemsFor(Array items, object selected)
    {
        onItemSelected = delegate { };
        itemRect.Value.DestroyChildren();
        itemsObjects = new();
        foreach (var value in items)
        {
            var item = Instantiate(templateItem.Value, itemRect);
            var label = item.transform.Find("Label").GetComponent<TextMeshProUGUI>();
            label.text = value.ToString();
            item.gameObject.name = value.ToString()!;
            item.isOn = value.Equals(selected);
            item.onValueChanged = new();
            item.onValueChanged.AddListener((UnityAction<bool>)(_ =>
            {
                onItemSelected.Invoke(value);
                Close();
            }));
            
            item.gameObject.SetActive(true);
            itemsObjects.Add(item);
        }
    }

    [HideFromIl2Cpp]
    public void Show(ConfigEnumSetting setting)
    {
        current = setting;
        transform.position = current.button.Value.transform.position - new Vector3(0, 60f, 0);
        gameObject.SetActive(true);
    }

    [HideFromIl2Cpp]
    public void Close()
    {
        current.expanded = false;
        current = null;
        onItemSelected = null;
        gameObject.SetActive(false);
    }

    public void Update()
    {
        if (current)
        {
            transform.position = current.button.Value.transform.position - new Vector3(0, 60f, 0);
            
            if (ClickOut())
            {
                Close();
            }
        }
    }
    
    
    private bool ClickOut()
    {
        if (!Input.GetMouseButtonDown(0))
            return false;

        var eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            var toggle = result.gameObject.GetComponentInParent<Toggle>();
            var button = result.gameObject.GetComponentInParent<Button>();
            if (toggle && (itemsObjects.Contains(toggle) || current.button.Value == button))
                return false;
        }

        return true;
    }
}