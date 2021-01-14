using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public abstract class Enemy : MonoBehaviour
{
    // Combat
    public GameObject bullet;
    public GameObject player;

    // Graphics
    public GameObject body;
    public Texture normalTex;
    protected Renderer rend;
    protected Rigidbody rb;
    protected TrailRenderer tr;
    protected Texture flashTex;

    protected Vector3 targetLocation;

    //Stats
    public float moveSpeed = 10f;
    public int maxHealth;
    protected int curHealth; // number of times the enemy can hit the walls before exploding
    public float flashTime = 4;
    protected float flashCounter = 0;
    protected bool isFlashing = false;

    protected bool isAlive = true;

    // Destruction
    public List<GameObject> scraps;
    public GameObject explosionPrefab;

    private int[] numOfScraps; // visual: the number of small pieces the enemy will spawn when it dies
    void Awake()
    {
        targetLocation = new Vector3(0, 8, 0);
        curHealth = maxHealth;
        numOfScraps = new int[scraps.Count];
        for (int i = 0; i < scraps.Count; i++)
        {
            Scraps temp = scraps[i].GetComponent<Scraps>();
            numOfScraps[i] = temp.GenerateAmount();
        }
        rend = body.GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<TrailRenderer>();
    }

    IEnumerator Behavior()
    {
        //Implement at children
        yield return new WaitForSeconds(0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Unpassable" && isAlive && !isFlashing)
        {
            // when colliding on the wall, damage the enemy.
            Damage();
        }
        else if (collision.gameObject.tag == "Enemy" && isAlive && !isFlashing)
        {
            Damage();
        }
    }

    void Damage()
    {
        FindObjectOfType<ComboManager>().AddCombo();
        curHealth--;
        Flash();
        if (curHealth <= 0)
        {
            // destroy this object and spawn scraps (visual only)
            isAlive = false;
            Explode();
        }
    }

    void Flash()
    {
        isFlashing = true;
        rend.material.SetTexture("_MainTex", flashTex);
    }

    // Handle Destruction when enemy is 0 HP:
    // Create an explosion prefab and scraps.
    void Explode()
    {
        FindObjectOfType<AudioManager>().Play("Explosion1");
        //FindObjectOfType<AudioManager>().Play("Explosion2");
        // creates small scraps
        GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation) as GameObject;
        ParticleSystem parts = explosion.GetComponent<ParticleSystem>();
        float totalDuration = parts.main.duration;
        Destroy(explosion, totalDuration);

        for (int i = 0; i < scraps.Count; i++)
        {
            for (int j = 0; j < numOfScraps[i]; j++)
            {
                GameObject scrap = scraps[i];
                GameObject createdScrap = Instantiate(scrap, transform.position, Quaternion.identity) as GameObject;
                // each scrap has random size and rotation
                float size = scrap.GetComponent<Scraps>().GenerateSize();
                scrap.transform.localScale = new Vector3(size, size, size);
                Quaternion rotationTarget = Quaternion.Euler(Random.Range(0, 359), Random.Range(0, 359), Random.Range(0, 359));
                scrap.transform.rotation = Quaternion.Slerp(scrap.transform.rotation, rotationTarget, 0);
            }
        }
        Destroy(gameObject);
    }


    // Update is called once per frame
    protected virtual void Update()
    {
        if (rb.velocity.magnitude > 20f)
            tr.enabled = true;
        else
            tr.enabled = false;
        if (isFlashing)
        {
            flashCounter++;
            if (flashCounter >= flashTime)
            {
                rend.material.SetTexture("_MainTex", normalTex);
                isFlashing = false;
                flashCounter = 0;
            }
        }
    }
}
