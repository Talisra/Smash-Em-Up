using UnityEngine;

public class ComboPerk : Perk
{
    public override void ChangeProfileAttributes(Profile profile)
    {
        switch (currentPerkLevel)
        {
            case 1:
                {
                    profile.maxCombo = 5;
                    break;
                }
            case 2:
                {
                    profile.maxCombo = 10;
                    break;
                }
        }
        base.ChangeProfileAttributes(profile);

    }
}
