using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DurableShield : Perk
{
    public override void ChangeProfileAttributes(Profile profile)
    {
        switch (currentPerkLevel)
        {
            case 1:
                {
                    profile.extraShield = .5f;
                    break;
                }
            case 2:
                {
                    profile.extraShield = 1;
                    break;
                }
            case 3:
                {
                    profile.extraShield = 2;
                    break;
                }
            case 4:
                {
                    profile.extraShield = 3;
                    break;
                }
            case 5:
                {
                    profile.extraShield = 5;
                    break;
                }
            default:
                break;
        }
        base.ChangeProfileAttributes(profile);

    }
}
