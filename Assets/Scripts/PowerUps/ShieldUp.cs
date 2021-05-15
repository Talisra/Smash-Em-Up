using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldUp : PowerUp
{
    private float duration = 3;
    public void SetDuration(int newDuration)
    {
        duration = newDuration;
    }
    public override void Collect()
    {
        AudioManager.Instance.Play(pickSoundString);
        player.GainShield(duration);
        Despawn();
    }

    public override void BackToPool()
    {
        ShieldUpPool.Instance.ReturnToPool(this);
    }
}
