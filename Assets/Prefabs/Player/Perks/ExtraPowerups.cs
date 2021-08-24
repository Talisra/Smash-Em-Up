using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraPowerups : Perk
{
    public override void ChangeProfileAttributes(Profile profile)
    {
        switch (currentPerkLevel)
        {
            case 1:
                {
                    profile.extraPowerUps = 1;
                    break;
                }
            case 2:
                {
                    profile.extraPowerUps = 2;
                    break;
                }
            case 3:
                {
                    profile.extraPowerUps = 3;
                    break;
                }
            case 4:
                {
                    profile.extraPowerUps = 4;
                    break;
                }
            case 5:
                {
                    profile.extraPowerUps = 5;
                    break;
                }
        }
        base.ChangeProfileAttributes(profile);

    }
}
