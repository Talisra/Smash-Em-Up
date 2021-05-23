using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBullet : MonoBehaviour
{
    //public GameObject player;
    public GameObject fireOutAnimation;
    public GameObject destroyAnimation;
    public float speed = 5000;
    private float radialSpeed = 100;
    private Player player;
    private Transform dest;
    private Rigidbody rb;
    private bool isDead = false;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        isDead = false;
        if (FindObjectOfType<GameManager>().CheckGameOver())
            return;
        AudioManager.Instance.Play("CubulletFired");
        dest = (transform.position.y > player.transform.position.y) ?
            player.GetWeakPointTop() : player.GetWeakPointBot();
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
        Explode();
    }

    private void Update()
    {
        Quaternion targetRot = Quaternion.Euler(0, 0, radialSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 0);
    }

    public void Explode()
    {
        if (!isDead)
        {
            isDead = true;
            AudioManager.Instance.Play("CubulletDestroy");
            GameObject exp = Instantiate(
                destroyAnimation, transform.position, Quaternion.identity) as GameObject;
            Destroy(exp, exp.GetComponent<ParticleSystem>().main.duration);
            rb.velocity = Vector3.zero;
            BasicBulletPool.Instance.ReturnToPool(this);
        }
    }
}
