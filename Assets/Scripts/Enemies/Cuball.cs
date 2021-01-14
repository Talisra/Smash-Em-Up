using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cuball : Enemy
{
    public GameObject windupAnimationPrefab;
    public GameObject bulletPrefab;

    private void Start()
    {
        GenerateTargetLocation();
        StartCoroutine(Behavior());
    }

    IEnumerator Behavior()
    {
        while(true)
        {
            yield return new WaitForSeconds(1);
            ChargeAttack();
            yield return new WaitForSeconds(6.5f);
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
        Destroy(windup, 2.3f);
        Invoke("Shoot", 2.3f);
    }

    private void Shoot()
    {
        Instantiate(bulletPrefab, transform.position, Quaternion.identity);
    }

    private void GenerateTargetLocation()
    {
        List<float> availableArea = FindObjectOfType<GameManager>().GetGameArea();
        targetLocation = new Vector3(
            Random.Range(availableArea[0],availableArea[2]),
            Random.Range(availableArea[1], availableArea[3]),
            0);
    }

    protected override void Update()
    {
        base.Update();
    }

}
