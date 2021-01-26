using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalScrap : Scraps
{
    public GameObject explosionPrefab;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Scrap")
            Explode();
    }

    void Explode()
    {
        audioManager.Play("TinyShatter");
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity) as GameObject;
        Destroy(explosion, explosion.GetComponent<ParticleSystem>().main.duration);
        Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1,1));
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -15)
            Destroy(gameObject);
    }
}
