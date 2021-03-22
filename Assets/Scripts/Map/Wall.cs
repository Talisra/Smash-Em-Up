using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public GameObject hitParticle;
    private AudioManager audioManager;
    // Start is called before the first frame update

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            HitWall(collision.transform.position);
        }
    }

    void HitWall(Vector3 coordinates)
    {
        Instantiate(hitParticle, coordinates, Quaternion.identity);
        audioManager.Play("Wall_Impact");
    }

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
