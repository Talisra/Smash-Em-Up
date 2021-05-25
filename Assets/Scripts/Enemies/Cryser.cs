using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cryser : Enemy
{
    public GameObject windupAnimationPrefab;

    public GameObject rightLaser;
    public GameObject leftLaser;

    public string chargeAudio;
    public string fireAudio;

    private float fireTime = 0.8f;

    // Start is called before the first frame update
    void Start()
    {
        rb.maxAngularVelocity = 2; // Do not let Cryser rotate too much - easier to deal with as an enemy.
        StartCoroutine(Behavior());
    }

    protected override IEnumerator Behavior()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);
            ChargeLaser();
            yield return new WaitForSeconds(5.3f);
        }
    }

    public void ChargeLaser()
    {
        AudioManager.Instance.Play(chargeAudio);
        GameObject windup = Instantiate(
            windupAnimationPrefab, transform.position, Quaternion.identity) as GameObject;
        windup.transform.SetParent(this.transform);
        ParticleSystem windupParticle = windup.GetComponent<ParticleSystem>();
        //float delay = windupParticle.main.simulationSpeed * windupParticle.main.duration;
        Destroy(windup, 2.0f);
        Invoke("StartFire", 1.5f);
    }

    public void StartFire()
    {
        AudioManager.Instance.Play(fireAudio);
        rightLaser.SetActive(true);
        leftLaser.SetActive(true);
        Invoke("StopFire", fireTime);
    }

    public void StopFire()
    {
        rightLaser.SetActive(false);
        leftLaser.SetActive(false);
    }

    public override void BackToPool()
    {
        base.BackToPool();
        CryserPool.Instance.ReturnToPool(this);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
