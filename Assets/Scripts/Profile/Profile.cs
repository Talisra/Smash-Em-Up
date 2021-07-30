[System.Serializable]
public class Profile
{
    public int level { get; set; }
    public int currentExp;
    public int expToNext;
    public string profileName { get; set; }

    private int storedExp;

    public Skill[] skills;

    public void GainExp(int amount)
    {
        storedExp += amount;
    }
}
