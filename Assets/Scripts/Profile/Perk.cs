using UnityEngine;

public class Perk : MonoBehaviour
{
    public string perkName;
    public Sprite image;
    public string description;
    public bool isSkill; // for active skills
    public Skill skill; // actual Skill script
    [HideInInspector]
    public int skillIndexAtPerksManager;
    public int currentPerkLevel = 0;
    public int[] levelsAquired;
    public string[] levelDescriptions;

    public string CreateFullDescription()
    {
        string rv = description + "\n";
        rv += "Level " +(currentPerkLevel) +": " +levelDescriptions[currentPerkLevel-1] + "\n";
        return rv;
    }

    public virtual void ChangeProfileAttributes(Profile profile)
    {
        MenuManager.Instance.SaveProfiles();
    }
}
