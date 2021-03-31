using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public int type; // type defines the wall type: 0: Wall, 1: Ceiling
    public GameObject hitParticle;
    private AudioManager audioManager;
    // Start is called before the first frame update

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            HitWall(collision.transform.position);
        }
        if (collision.gameObject.tag == "Player")
        {
            audioManager.Play("MetalCollision");
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
