using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinibossScraps : Scraps
{
    public GameObject explosionPrefab;

    private float radialSpeed = 0f;
    private float accel = 1f;

    private void OnEnable()
    {
        float exploDelay = Random.Range(1.5f, 2.5f);
        GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-5,5), Random.Range(0,10), Random.Range(-1,1)), ForceMode.VelocityChange);
        Invoke("Explode", exploDelay);
    }

    private void Explode()
    {
        if (isActiveAndEnabled)
        {
            AudioManager.Instance.Play("Explosion_Evaporate");
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity) as GameObject;
            Destroy(explosion, explosion.GetComponent<ParticleSystem>().main.duration);
            BackToPool();
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.5f, 0.5f, 0.5f), 1 * Time.deltaTime);
        radialSpeed += accel;
        transform.Rotate(0, radialSpeed * Time.deltaTime, 0, 0);
    }
}
