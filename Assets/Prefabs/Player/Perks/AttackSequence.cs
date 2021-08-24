using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSequence : Perk
{
    public override void ChangeProfileAttributes(Profile profile)
    {
        switch (currentPerkLevel)
        {
            case 2:
                {
                    profile.atkSeqCap = 1;
                    break;
                }
            case 3:
                {
                    profile.atkSeqCap = 2;
                    break;
                }
            case 4:
                {
                    profile.atkSeqCap = 3;
                    break;
                }
            default:
                break;
        }
        base.ChangeProfileAttributes(profile);

    }
}
