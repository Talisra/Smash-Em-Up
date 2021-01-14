using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    public CanvasGroup myCG;
    private bool flash = false;
    void Update()
    {
        if (flash)
        {
            myCG.alpha = myCG.alpha - Time.deltaTime*1.5f;
            if (myCG.alpha <= 0)
            {
                myCG.alpha = 0;
                flash = false;
            }
        }
    }

    public void FlashDamage()
    {
        FindObjectOfType<AudioManager>().Play("PlayerDamage");
        flash = true;
        myCG.alpha = 1;
    }
}
