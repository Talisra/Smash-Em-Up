using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class DraggableSkill : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public CanvasGroup canvasGroup;
    public Perk perk;
    public Image bg;
    public Image icon;
    public Text perkName;
    public Text lvl;

    public RectTransform rectTransform;

    [HideInInspector]
    public PerkPanelUI parentUI;

    public void InitPerk(Perk newPerk)
    {
        perk = newPerk;
        icon.sprite = perk.image;
        perkName.text = perk.perkName;
        lvl.text = "Lvl. " + newPerk.currentPerkLevel.ToString();
        bg.color = Color.red;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
    }
    
    public void DragProperties()
    {
        rectTransform.transform.position = Input.mousePosition;
        canvasGroup.alpha = .7f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        gameObject.SetActive(false);
        parentUI.DragRelease();
    }

    public void SetParentUI(PerkPanelUI panel)
    {
        parentUI = panel;
    }
}
