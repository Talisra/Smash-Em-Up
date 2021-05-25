using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unpassable : MonoBehaviour
{
    public int type; // type defines the wall type: 0: Wall, 1: Ceiling, 2: floor
    public GameObject slamParticle;
    public string slamSound;
    public string hitSound;

    // Start is called before the first frame update

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            HitWall(collision.transform.position);
        }
        if (collision.gameObject.tag == "Player")
        {
            AudioManager.Instance.Play("MetalCollision");
        }
    }

    void HitWall(Vector3 coordinates)
    {
        AudioManager.Instance.Play(hitSound);
    }

    public void SlamWall(Vector3 coordinates)
    {
        AudioManager.Instance.Play(slamSound);
        GameObject dust = Instantiate(slamParticle, coordinates, Quaternion.identity);
        ParticleSystem parts = dust.GetComponent<ParticleSystem>();
        float totalDuration = parts.main.duration;
        Destroy(dust, totalDuration);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
