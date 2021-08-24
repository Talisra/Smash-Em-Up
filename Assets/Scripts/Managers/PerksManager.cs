using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerksManager : MonoBehaviour
{
    public static PerksManager Instance { get; private set; }

    public List<Perk> allPerks = new List<Perk>(); // all the perks must be added here! the order doesnt matter

    public List<Perk> totalSkills = new List<Perk>(); // all skills will be here for equip

    [HideInInspector]
    public List<Perk> perksToShow = new List<Perk>(); // perks to show on the perk screen
    [HideInInspector]
    public List<Perk> skills = new List<Perk>(); // storage of skills

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        foreach(Perk perk in allPerks)
        {
            if (perk.isSkill)
            {
                totalSkills.Add(perk);
            }
        }
        for (int i = 0; i < totalSkills.Count; i++)
        {
            totalSkills[i].skillIndexAtPerksManager = i;
        }
    }

    public Skill GetSkillByIndex(int index)
    {
        if (index == -1)
            return null;
        return totalSkills[index].skill;
    }

    public void CheckNewPerks(int level)
    {
        foreach (Perk perk in allPerks)
        {
            for (int i = 0; i < perk.levelsAquired.Length; i++)
            {
                if (level == perk.levelsAquired[i])
                {
                    perk.currentPerkLevel = i+1;
                    perk.ChangeProfileAttributes(GameProfile.Instance.GetProfile());
                    perksToShow.Add(perk);
                }
            }
        }
    }

    public void GetAllPerksByLevel(int level) // if the bool is set to true, will get perks, if set to false, will get skills
    {
        foreach (Perk perk in allPerks)
        {
            for (int i = perk.levelsAquired.Length-1; i >= 0; i--)
            {
                if (level >= perk.levelsAquired[i])
                {
                    perk.currentPerkLevel = i + 1;
                    if (perk.isSkill)
                    {
                        skills.Add(perk);
                    }
                    else
                    {
                        perksToShow.Add(perk);
                    }
                    i = 0;  
                }
            }
        }
    }

}
