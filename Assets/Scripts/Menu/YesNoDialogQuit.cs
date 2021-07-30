using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YesNoDialogQuit : YesNoDialog
{
    public InGameMenu parentMenu;

    public override void Action(int action)
    {
        switch (action)
        {
            case 0: // yes
                {
                    gameObject.SetActive(false);
                    parentMenu.Action(6);
                    break;
                }
            case 1: // no
                {
                    gameObject.SetActive(false);
                    break;
                }
            default:
                break;
        }
    }
}
