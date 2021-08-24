using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerVelocity : Perk
{
    public override void ChangeProfileAttributes(Profile profile)
    {
        switch (currentPerkLevel)
        {
            case 1:
                {
                    profile.extraSpeed = 2;
                    break;
                }
            case 2:
                {
                    profile.extraSpeed = 4;
                    break;
                }
            case 3:
                {
                    profile.extraSpeed = 6;
                    break;
                }
            case 4:
                {
                    profile.extraSpeed = 8;
                    break;
                }
            default:
                break;
        }
        base.ChangeProfileAttributes(profile);

    }
}
