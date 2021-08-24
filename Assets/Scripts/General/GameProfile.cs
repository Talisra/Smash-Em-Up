using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProfile : MonoBehaviour
{
    public static GameProfile Instance { get; private set; }

    [HideInInspector]
    public bool isLevel;

    public bool muteSound = false;
    public bool muteMusic = false;

    private Profile profile;
    private float expPool = 0; // stores the exp to give it to the profile after the game end
    public Skill leftMouseSkill = null;
    public Skill rightMouseSkill = null;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void SetSkills(Skill lmb, Skill rmb)
    {
        leftMouseSkill = lmb;
        rightMouseSkill = rmb;
    }

    public Skill[] GetSkills()
    {
        return new Skill[] { leftMouseSkill, rightMouseSkill };
    }

    public void ShowTutorialForPlayer(int type) // show tutorial screen if the player hasn't see it yet
    {
        switch (type)
        {
            case 0: // Start Game for the first time
                {
                    if (profile.tutStartGame)
                        return;
                    GameManager.Instance.ShowTutorialScreen(0);
                    profile.tutStartGame = true;
                    break;
                }
            case 1: // Skill Window can be clicked
                {
                    if (profile.tutSkillsClickable)
                        return;
                    MenuManager.Instance.ShowTutorialScreen(1);
                    profile.tutSkillsClickable = true;
                    break;
                }
            case 2: // What are power ups? + // how to score a combo and what is the reward
                {
                    if (profile.tutCombo)
                        return;
                    GameManager.Instance.ShowTutorialScreen(2);
                    profile.tutCombo = true;
                    break;
                }
            case 3: // explanation on skill observer and skill equip
                {
                    if (profile.tutSkillWindow)
                        return;
                    MenuManager.Instance.ShowTutorialScreen(3);
                    profile.tutSkillWindow = true;
                    break;
                }
            case 4: // explanation on skillpanelUI
                {
                    if (profile.tutSkill)
                        return;
                    MenuManager.Instance.ShowTutorialScreen(4);
                    profile.tutSkill = true;
                    break;
                }
            case 5: // explanation on taking damage
                {
                    if (profile.tutDamage)
                        return;
                    GameManager.Instance.ShowTutorialScreen(5);
                    profile.tutDamage = true;
                    break;
                }
        }
    }

    public void SetLevelAndCheckProfile()
    {
        isLevel = true;
        if (profile == null)
        {
            profile = new Profile("Demo", 4);
        }
    }

    public Profile GetProfile()
    {
        return profile;
    }

    public void SetProfile(Profile prof)
    {
        if (prof == null)
            profile = null;
        if (prof != null)
        {
            if (prof.level == 0)
            {
                profile = null;
            }
            else
                profile = prof;
        }
    }

    public float GetExpPool()
    {
        return expPool;
    }

    public void AddExpToPool(int amount)
    {
        expPool += amount;
    }

    public void ClearExpPool()
    {
        expPool = 0;
    }

}
