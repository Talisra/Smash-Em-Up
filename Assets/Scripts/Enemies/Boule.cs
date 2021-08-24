using UnityEngine;

public class Boule : Enemy
{
    private bool hitByPlayer = false;

    protected override void OnEnable()
    {
        base.OnEnable();
        hitByPlayer = inGame ? true : false;
    }

    public override void HitByPlayer()
    {
        base.HitByPlayer();
        hitByPlayer = true;
    }

    public override void Damage(int amount)
    {
        if (hitByPlayer)
            base.Damage(amount);
    }

    public override void BackToPool()
    {
        base.BackToPool();
        BoulePool.Instance.ReturnToPool(this);
    }
}
