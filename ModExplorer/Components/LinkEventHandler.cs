// HEAVILY modified TMP_TextEventHandler from TextMeshPro Extras
// All credits to Unity
using UnityEngine;
using System;
using TMPro;

namespace ModExplorer.Components;

public class LinkEventHandler(IntPtr cppPtr) : MonoBehaviour(cppPtr)
{
    public TMP_Text textComponent;
    public Camera camera;
    public Canvas canvas;

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
            if (Input.GetMouseButtonDown(0))
            {
                TMP_LinkInfo linkInfo = textComponent.textInfo.linkInfo[linkIndex];
                Application.OpenURL(linkInfo.GetLinkID());
            }
        }
    }
}