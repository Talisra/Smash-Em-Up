using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaulPerk : Perk
{
    public override void ChangeProfileAttributes(Profile profile)
    {
        switch (currentPerkLevel)
        {
            case 1:
                {
                    profile.maulMinSpeed = 150;
                    break;
                }
            case 2:
                {
                    profile.maulMinSpeed = 125;
                    break;
                }
            default:
                break;
        }
        base.ChangeProfileAttributes(profile);

    }
}
