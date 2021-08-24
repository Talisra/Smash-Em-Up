using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trinati : Enemy
{
    public List<TrinatiShield> shields;
    public List<Transform> shootingPoints;
    private float canShootCounter = 0;
    private float shotDelay = 0.33f;

    private bool isInShootMode = false;
    private float singleShotCounter = 0;
    private float singleShotDelay = .33f;
    private int numOfShots = 3;
    private int shotsCounter = 0;

    protected override void OnEnable()
    {
        base.OnEnable();
        foreach(TrinatiShield shield in shields)
        {
            shield.gameObject.SetActive(true);
        }
        shotsCounter = 0;
        canShootCounter = 0;
        singleShotCounter = 0;
        isInShootMode = false;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (rb.velocity.magnitude > minVelocityForDamage) // damage is only possible when the enemy has some acceleration
        {
            if (isAlive && rb.velocity.magnitude > minVelocityForDamage)
            {
                if (collision.gameObject.GetInstanceID() != lastCollisionID)
                {
                    if (collision.gameObject.tag == "Unpassable" || collision.gameObject.tag == "Enemy")
                    {
                        if (collision.contacts[0].thisCollider.gameObject.tag == "Enemy")
                        {
                            if (inCombo)
                            {
                                ResetComboChain();
                                comboManager.AddCombo();
                            }
                            Damage(isSuperSpeed ? 2 : 1);
                        }
                        else if (collision.contacts[0].thisCollider.gameObject.tag == "EnemyPart")
                        {
                            TrinatiShield shield = collision.contacts[0].thisCollider.gameObject.GetComponent<TrinatiShield>();
                            shield.Damage();
                        }
                    }
                }
            }
        }
        lastCollisionID = collision.gameObject.GetInstanceID();
    }

    private void ShootMultiDirections()
    {
        AudioManager.Instance.Play("TrinityBulletFire");
        for (int i = 0; i < shootingPoints.Count; i++)
        {
            TrinityBullet bullet = TrinityBulletPool.Instance.Get(transform.position, Quaternion.identity);
            bullet.SetPosition(shootingPoints[i].position - transform.position);
        }
    }

    protected override void Update()
    {
        base.Update();
        if (isInShootMode)
        {
            if (shotsCounter < numOfShots)
            {
                singleShotCounter += Time.deltaTime;
                if (singleShotCounter >= singleShotDelay)
                {
                    ShootMultiDirections();
                    singleShotCounter = 0;
                    shotsCounter++;
                }
            }
            else
            {
                shotsCounter = 0;
                isInShootMode = false;
            }
        }
        else
        {
            canShootCounter += Time.deltaTime;
            if (canShootCounter >= shotDelay)
            {
                canShootCounter = 0;
                isInShootMode = true;
            }
        }
    }

    public override void BackToPool()
    {
        base.BackToPool();
        TrinatiPool.Instance.ReturnToPool(this);
    }
}
