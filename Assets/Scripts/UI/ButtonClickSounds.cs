using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClickSounds : MonoBehaviour
{
    private void OnMouseEnter()
    {
        AudioManager.Instance.MenuPlay("Button_Hover");

    }

    private void OnMouseDown()
    {
        AudioManager.Instance.MenuPlay("Button_Click");
    }
}
