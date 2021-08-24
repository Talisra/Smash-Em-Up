using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraHP : Perk
{
    public override void ChangeProfileAttributes(Profile profile)
    {
        switch (currentPerkLevel)
        {
            case 1:
                {
                    profile.extraHP = 1;
                    break;
                }
            case 2:
                {
                    profile.extraHP = 2;
                    break;
                }
            case 3:
                {
                    profile.extraHP = 3;
                    break;
                }
            case 4:
                {
                    profile.extraHP = 4;
                    break;
                }
            case 5:
                {
                    profile.extraHP = 5;
                    break;
                }
            case 6:
                {
                    profile.extraHP = 6;
                    break;
                }
            case 7:
                {
                    profile.extraHP = 7;
                    break;
                }
            case 8:
                {
                    profile.extraHP = 8;
                    break;
                }
            case 9:
                {
                    profile.extraHP = 9;
                    break;
                }
            case 10:
                {
                    profile.extraHP = 10;
                    break;
                }
            default:
                break;
        }
        base.ChangeProfileAttributes(profile);

    }
}
