using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YesNoDialogDeleteProfile : YesNoDialog
{
    public HoloProfileButton button;

    public override void Action(int action)
    {
        switch (action)
        {
            case 0: // yes
                {
                    button.DeleteProfile();
                    gameObject.SetActive(false);
                    break;
                }
            case 1: // no
                {
                    button.selector.gameObject.SetActive(true);
                    gameObject.SetActive(false);
                    break;
                }
            default:
                break;
        }
    }
}
