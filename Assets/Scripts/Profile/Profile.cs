[System.Serializable]
public class Profile
{
    // basic attributes
    public int level = 1;
    public string profileName { get; set; }
    public int slot;

    public bool muteSound;
    public bool muteMusic;

    public int currentExp = 0;
    public int expToNext = 0;

    public int skillLevel = 0;

    // perks
    public int atkSeqCap = 0;
    public int extraHP = 0;
    public float extraShield = 0;
    public float extraSpeed = 0;
    public int maxCombo = 0;
    public int extraPowerUps = 0;
    public int extraHeal = 0;
    public int maulMinSpeed = 1000;

    // skills
    public int activeSkillMouseLeft = -1;
    public int activeSkillMouseRight = -1;

    // tutorials
    public bool tutStartGame = false;
    public bool tutSkillsClickable = false;
    public bool tutCombo = false;
    public bool tutSkillWindow = false;
    public bool tutSkill = false;
    public bool tutDamage = false;

    public Profile(string name, int profileSlot)
    {
        profileName = name;
        slot = profileSlot;
        expToNext = EXPtable.ExperienceTable[level];
    }

    public void LevelUP()
    {
        level++;
        CheckPerks();
        currentExp = 0;
        expToNext = EXPtable.ExperienceTable[level];
    }


    public void CheckPerks()
    {
        PerksManager.Instance.CheckNewPerks(level);
    }
}
