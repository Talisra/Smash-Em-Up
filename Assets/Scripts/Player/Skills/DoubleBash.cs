using UnityEngine;

public class DoubleBash : Skill
{
    public override void OnSmash(Enemy enemy)
    {
        if (!enemy.isHit)
        {
            enemy.HitByPlayer();
            CameraEffects.Shake(0.2f, 0.2f);
            AudioManager.Instance.Play("Bash");
            // calculate the distance to know if animation is from left or right
            Vector3 enemyPos = enemy.transform.position;
            Vector3 playerPos = transform.position;
            float enemyDirection = -playerPos.x * enemyPos.y + playerPos.y * enemyPos.x; // negative = left, positive = right
            ShowHitParticle(enemy.transform.position);
            Rigidbody rbenemy = enemy.GetComponent<Rigidbody>();
            Vector3 PowerVector = new Vector3(
                Mathf.Sign(enemyDirection) * 4000,
                0, 0);
            rbenemy.AddForce(PowerVector);
            enemy.GetComponent<Enemy>().GiveSuperSpeed(0.3f);
        }
    }

    public override void OnSmashVoid(Vector3 position)
    {
        CameraEffects.Shake(0.2f, 0.2f);
        AudioManager.Instance.Play("Bash");
        Vector3 playerPos = transform.position;
        float direction = -playerPos.x * position.y + playerPos.y * position.x; // negative = left, positive = right
        ShowHitParticle(position);
    }

    public override void OnStartAction()
    {
        AudioManager.Instance.Play("Bashwu");
        player.animator.Play(animationName);
        player.head.EnableAttackEffect();
    }

    public override void OnEndAction()
    {
        player.head.DisableAttackEffect();

    }
}
