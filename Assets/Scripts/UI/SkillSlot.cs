using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillSlot : MonoBehaviour, IDropHandler, IPointerDownHandler
{
    public PerkPanelUI panel;
    private Perk activePerk = null;

    public Perk GetPerk()
    {
        return activePerk;
    }

    public void EquipPerk(Perk perk)
    {
        activePerk = perk;
        if (activePerk == null)
        {
            if (panel.description_tail)
                panel.description_tail.gameObject.SetActive(false);
            panel.gameObject.SetActive(false);
        }
        else
        {
            panel.gameObject.SetActive(true);
            panel.isInSlot = true;
            panel.InitPerk(perk);
        }

    }

    public void OnDrop(PointerEventData eventData)
    {
        DraggableSkill skill;
        if (eventData.pointerDrag.TryGetComponent<DraggableSkill>(out skill))
            EquipPerk(skill.perk);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            EquipPerk(null);
        }
    }
}
