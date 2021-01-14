using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBullet : MonoBehaviour
{
    //public GameObject player;
    public GameObject fireOutAnimation;
    public GameObject destroyAnimation;
    public float speed = 1000;
    private float radialSpeed = 100;
    private Transform dest;
    private Rigidbody rb;

    private void Start()
    {
        if (FindObjectOfType<GameManager>().CheckGameOver())
            return;
        FindObjectOfType<AudioManager>().Play("CubulletFired");
        Player playerScript = FindObjectOfType<Player>().GetComponent<Player>();
        rb = GetComponent<Rigidbody>();
        dest = (transform.position.y > playerScript.transform.position.y) ?
            playerScript.GetWeakPointTop() : playerScript.GetWeakPointBot();
        //dest = player.transform.position;
        //dest = player
        GameObject fired = Instantiate(
            fireOutAnimation,
            transform.position,
            Quaternion.Euler(0,0, Quaternion.Angle(transform.rotation, dest.rotation))
            ) as GameObject;
        Destroy(fired, fired.GetComponent<ParticleSystem>().main.duration);
        rb.AddForce((dest.position - transform.position) * speed, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Unpassable")
        {
            Explode();
        }

    }

    private void Update()
    {
        Quaternion targetRot = Quaternion.Euler(0, 0, radialSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 0);
    }

    public void Explode()
    {
        FindObjectOfType<AudioManager>().Play("CubulletDestroy");
        GameObject exp = Instantiate(
            destroyAnimation, transform.position, Quaternion.identity) as GameObject;
        Destroy(exp, exp.GetComponent<ParticleSystem>().main.duration);
        Destroy(gameObject);
    }
}
