using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinibossBullet : MonoBehaviour
{
    public GameObject explodeAnimation;
    private Rigidbody rb;

    private bool exploded = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Shoot(Vector3 direction, float speed)
    {
        rb.AddForce(direction.normalized * speed, ForceMode.VelocityChange);
    }

    private void OnEnable()
    {
        exploded = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(1, true, true, transform.position);
        }
        Explode();
    }

    public void Explode()
    {
        if (!exploded)
        {
            exploded = true;
            AudioManager.Instance.Play("MiniBossBulletDestroy");
            GameObject exp = Instantiate(
                explodeAnimation, transform.position, Quaternion.identity) as GameObject;
            Destroy(exp, exp.GetComponent<ParticleSystem>().main.duration);
            rb.velocity = Vector3.zero;
            MinibossBullet_Pool.Instance.ReturnToPool(this);
        }
    }
}
