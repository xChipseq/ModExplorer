// HEAVILY modified TMP_TextEventHandler from TextMeshPro Extras
// All credits to Unity
using UnityEngine;
using System;
using Il2CppInterop.Runtime.Attributes;
using TMPro;

namespace ModExplorer.Components;

public class LinkEventHandler(IntPtr cppPtr) : MonoBehaviour(cppPtr)
{
    public TMP_Text textComponent;
    public Camera camera;
    public Canvas canvas;
    public int lastLinkIdx = -1;
    
    [HideFromIl2Cpp]
    public event Action<string> onLinkClicked = delegate {};
    [HideFromIl2Cpp]
    public event Action<string> onLinkFocusLost = delegate {};

    public void Awake()
    {
        textComponent = gameObject.GetComponent<TMP_Text>();
        canvas = gameObject.GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        }
    }

    public void LateUpdate()
    {
        if (TMP_TextUtilities.IsIntersectingRectTransform(textComponent.rectTransform, Input.mousePosition, camera))
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(textComponent, Input.mousePosition, camera);
            if (linkIndex != -1)
            {
                lastLinkIdx = linkIndex;
                TMP_LinkInfo linkInfo = textComponent.textInfo.linkInfo[linkIndex];
                if (Input.GetMouseButtonDown(0))
                {
                    onLinkClicked.Invoke(linkInfo.GetLinkID());
                }
            }
            else if (lastLinkIdx != -1)
            {
                TMP_LinkInfo linkInfo = textComponent.textInfo.linkInfo[lastLinkIdx];
                onLinkFocusLost.Invoke(linkInfo.GetLinkID());
                lastLinkIdx = -1;
            }
        }
    }
}