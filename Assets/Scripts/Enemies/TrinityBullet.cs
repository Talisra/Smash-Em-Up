using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrinityBullet : MonoBehaviour
{
    public GameObject destroyAnimation;
    public Rigidbody rb;
    public float bulletSpeed = 10000;

    private bool isDead;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z < -20)
            Explode();
    }

    private void OnEnable()
    {
        isDead = false;
    }

    public void SetPosition(Vector3 position)
    {
        rb.AddForce((position.normalized) * bulletSpeed, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Unpassable" || collision.gameObject.tag == "Floor")
        {
            Explode();
        }
    }

    public void Explode()
    {
        if (!isDead)
        {
            isDead = true;
            AudioManager.Instance.Play("TrinityBulletDie");
            GameObject exp = Instantiate(
                destroyAnimation, transform.position, Quaternion.identity) as GameObject;
            Destroy(exp, 0.25f);
            rb.velocity = Vector3.zero;
            TrinityBulletPool.Instance.ReturnToPool(this);
        }
    }
}
