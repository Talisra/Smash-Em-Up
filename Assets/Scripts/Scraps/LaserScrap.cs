﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScrap : Scraps
{
    public GameObject explosionPrefab;

    void Explode()
    {
        audioManager.Play("TinyShatter");
        GameObject explosion1 = Instantiate(explosionPrefab, 
            transform.position + new Vector3(-0.5f, 0, 0), Quaternion.identity) as GameObject;
        Destroy(explosion1, explosion1.GetComponent<ParticleSystem>().main.duration);
        GameObject explosion2 = Instantiate(explosionPrefab,
            transform.position + new Vector3(0.5f, 0, 0), Quaternion.identity) as GameObject;
        Destroy(explosion2, explosion2.GetComponent<ParticleSystem>().main.duration);
        Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(0.3f, 0.6f, 0.3f);
        Invoke("Explode", Random.Range(0.8f, 1.4f));
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -15)
            Destroy(gameObject);
    }
}
