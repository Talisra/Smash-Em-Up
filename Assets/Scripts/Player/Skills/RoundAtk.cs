using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundAtk : Skill
{
    public override void OnSmash(Enemy enemy)
    {
        if (!enemy.isHit)
        {
            enemy.HitByPlayer();
            CameraEffects.Shake(0.25f, 0.3f);
            AudioManager.Instance.Play("SuperSmash");
            // calculate the distance to know if animation is from left or right
            Vector3 enemyPos = enemy.transform.position;
            Vector3 playerPos = transform.position;
            float enemyDirection = -playerPos.x * enemyPos.y + playerPos.y * enemyPos.x; // negative = left, positive = right
            ShowHitParticle(enemy.transform);
            Rigidbody rbenemy = enemy.GetComponent<Rigidbody>();
            Vector3 PowerVector = new Vector3(
                Mathf.Sign(enemyDirection) * 6000,
                0, 0);
            rbenemy.AddForce(PowerVector);
            enemy.GetComponent<Enemy>().GiveSuperSpeed(0.5f);
        }
    }

    public override void OnStartAction()
    {
        AudioManager.Instance.Play("RoundAtkwu");
        if (player.GetDeltaX() < 0)
            player.animator.Play(animationName + "Right");
        else
            player.animator.Play(animationName + "Left");
        player.head.EnableAttackEffect();
    }
    public override void OnEndAction()
    {
        player.head.DisableAttackEffect();
    }
}
