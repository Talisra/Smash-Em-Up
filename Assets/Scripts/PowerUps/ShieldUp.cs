using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldUp : PowerUp
{
    private float baseDuration = 5;
    public void SetDuration(int newDuration)
    {
        baseDuration = newDuration;
    }
    public override void Collect()
    {
        AudioManager.Instance.Play(pickSoundString);
        player.GainShield(baseDuration);
        Despawn();
    }

    public override void BackToPool()
    {
        ShieldUpPool.Instance.ReturnToPool(this);
    }
}
