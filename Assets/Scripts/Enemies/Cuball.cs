using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cuball : Enemy
{
    public GameObject windupAnimationPrefab;
    public GameObject bulletPrefab;

    public float minAtkRate = 0.7f;
    public float maxAtkRate = 1.3f;
    public float minIdleTime = 5.6f;
    public float maxIdleTime = 7.5f;
    protected override IEnumerator Behavior()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(minAtkRate, maxAtkRate));
            ChargeAttack();
            yield return new WaitForSeconds(Random.Range(minIdleTime, maxIdleTime));
        }
    }

    private void ChargeAttack()
    {
        //FindObjectOfType<AudioManager>().Play("CuballCharge");
        GameObject windup = Instantiate(
            windupAnimationPrefab, transform.position, Quaternion.identity) as GameObject;
        windup.transform.SetParent(this.transform);
        ParticleSystem windupParticle = windup.GetComponent<ParticleSystem>();
        //float delay = windupParticle.main.simulationSpeed * windupParticle.main.duration;
        Destroy(windup, 2.3f); // 2.3f is the animation windup time, no need to change!
        Invoke("Shoot", 2.3f);
    }

    private void Shoot()
    {
        if (isAlive)
            BasicBulletPool.Instance.Get(transform.position, Quaternion.identity);
    }

    public override void BackToPool()
    {
        base.BackToPool();
        CuballPool.Instance.ReturnToPool(this);
    }

    protected override void Update()
    {
        base.Update();
    }

}
