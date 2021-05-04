using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUp : PowerUp
{
    private int amount = 1;


    public void SetAmount(int newAmount)
    {
        amount = newAmount;
    }
    public override void Collect()
    {
        audioManager.Play(pickSoundString);
        player.Heal(amount);
        Despawn();
    }

    public override void BackToPool()
    {
        HealthUpPool.Instance.ReturnToPool(this);
    }

}
