﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleSmash : Perk
{
    public override void ChangeProfileAttributes(Profile profile)
    {
        switch (currentPerkLevel)
        {
            case 1:
                {
                    break;
                }
            default:
                break;
        }
        base.ChangeProfileAttributes(profile);
    }
}