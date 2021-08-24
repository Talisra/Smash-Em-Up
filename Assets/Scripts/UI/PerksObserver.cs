using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerksObserver : MonoBehaviour
{
    public Image perksList;
    public Image skillsList;

    public SkillSlot leftSkillSlot;
    public SkillSlot rightSkillSlot;

    public PerkPanelUI[] perkPanels;
    public PerkPanelUI[] skills;

    private Profile prof;

    [HideInInspector]
    public DraggableSkill draggableSkill;

    private void Awake()
    {
        prof = GameProfile.Instance.GetProfile();
        perkPanels = perksList.GetComponentsInChildren<PerkPanelUI>();
        skills = skillsList.GetComponentsInChildren<PerkPanelUI>();
    }

    public void DraggingSkill(DraggableSkill draggable)
    {
        draggableSkill = draggable;
    }

    public void SetProfile(Profile newprofile)
    {
        prof = newprofile;
    }
    private void OnEnable()
    {
        if (prof == null)
        {
            AddPerks(new List<Perk>());
            AddSkills(new List<Perk>());
        }
        else
        {
            PerksManager.Instance.GetAllPerksByLevel(prof.level);
            AddPerks(PerksManager.Instance.perksToShow);
            AddSkills(PerksManager.Instance.skills);
            PerksManager.Instance.perksToShow.Clear();
            PerksManager.Instance.skills.Clear();
            if (prof.activeSkillMouseLeft != -1)
                leftSkillSlot.EquipPerk(PerksManager.Instance.totalSkills[prof.activeSkillMouseLeft]);
            if (prof.activeSkillMouseRight != -1)
                rightSkillSlot.EquipPerk(PerksManager.Instance.totalSkills[prof.activeSkillMouseRight]);
            GameProfile.Instance.ShowTutorialForPlayer(3);
        }
    }

    public void Reset()
    {
        SetProfile(null);
        rightSkillSlot.EquipPerk(null);
        leftSkillSlot.EquipPerk(null);
    }

    private void OnDisable()
    {
        if (draggableSkill)
            draggableSkill.gameObject.SetActive(false);
        MenuManager.Instance.SaveProfiles();
        foreach(PerkPanelUI pui in skills)
        {
            pui.gameObject.SetActive(false);
        }
        foreach (PerkPanelUI pui in perkPanels)
        {
            pui.gameObject.SetActive(false);
        }
        MenuManager.Instance.UpdateProfileSkills(leftSkillSlot.GetPerk(), rightSkillSlot.GetPerk());
    }

    public void AddSkills(List<Perk> skill_list)
    {
        for (int i = 0; i < skill_list.Count; i++)
        {
            skills[i].gameObject.SetActive(true);
            skills[i].InitPerk(skill_list[i]);
        }
        foreach (PerkPanelUI pui in skills)
        {
            if (pui.perk == null)
                pui.gameObject.SetActive(false);
        }
    }

    public void AddPerks(List<Perk> perks_list)
    {
        for (int i = 0; i < perks_list.Count; i++)
        {
            perkPanels[i].gameObject.SetActive(true);
            perkPanels[i].InitPerk(perks_list[i]);
        }
        foreach(PerkPanelUI pui in perkPanels)
        {
            if (pui.perk == null)
                pui.gameObject.SetActive(false);
        }
    }

}
