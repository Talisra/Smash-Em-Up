using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public abstract class Enemy : MonoBehaviour, IPoolableObject
{
    // References
    public GameObject bullet;
    public GameObject player;

    protected AudioManager audioManager;
    protected Rigidbody rb;
    private ComboManager comboManager;
    private GameManager gameManager;

    // Graphics
    public GameObject body;
    public Texture normalTex;
    public Texture flashTex;
    protected Renderer rend;
    protected TrailRenderer tr;

    //Stats
    public float moveSpeed = 10f;
    public int maxHealth;
    protected int curHealth; // number of times the enemy can hit the walls before exploding
    public float flashTime = 4;
    protected float flashCounter = 0;
    protected bool isFlashing = false;
    protected bool isAlive = true;
    protected float minVelocityForDamage = 5f;

    //Combos
    private bool inCombo = false;
    private float comboCounter = 0;
    private float comboDelay = 2f;

    // Audio
    public string afterEffectAudio;
    public string explodeAudio;
    public string hitAudio;

    // Destruction
    public List<GameObject> scraps;
    public GameObject explosionPrefab;

    private int[] numOfScraps; // visual: the number of small pieces the enemy will spawn when it dies
    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        audioManager = FindObjectOfType<AudioManager>();
        numOfScraps = new int[scraps.Count];
        for (int i = 0; i < scraps.Count; i++)
        {
            Scraps temp = scraps[i].GetComponent<Scraps>();
            numOfScraps[i] = temp.GenerateAmount();
        }
        comboManager = FindObjectOfType<ComboManager>();
        rend = body.GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<TrailRenderer>();
    }

    private void OnEnable()
    {
        isAlive = true;
        curHealth = maxHealth;
        StartCoroutine(Behavior());
    }

    private void OnDisable()
    {
        //StopAllCoroutines();
    }

    protected virtual IEnumerator Behavior()
    {
        //Implement at children
        yield return new WaitForSeconds(0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (rb.velocity.magnitude > minVelocityForDamage) // damage is only possible when the enemy has some acceleration
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
    }

    public void ResetComboChain()
    {
        comboCounter = 0;
        inCombo = true;
    }

    public void HitByPlayer()
    {
        audioManager.Play(hitAudio);
    }

    void Damage()
    {
        if (inCombo)
        {
            ResetComboChain();
            comboManager.AddCombo();
        }
        audioManager.Play(hitAudio);
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
        CameraShake.Shake(0.3f, 0.4f);
        audioManager.Play(explodeAudio);
        if (afterEffectAudio != null)
            audioManager.Play(afterEffectAudio);
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
                GameObject createdScrap = ScrapsPooler.Instance.Get(
                    scraps[i].name, transform.position, Quaternion.identity) as GameObject;
                // each scrap has random size and rotation
                Vector3 size = scrap.GetComponent<Scraps>().GenerateSize();
                createdScrap.transform.localScale = size;
                Quaternion rotationTarget = Quaternion.Euler(Random.Range(0, 359), Random.Range(0, 359), Random.Range(0, 359));
                createdScrap.transform.rotation = Quaternion.Slerp(createdScrap.transform.rotation, rotationTarget, 0);
            }
        }
        BackToPool();
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
        if (inCombo)
        {
            comboCounter += Time.deltaTime;
            if (comboCounter >= comboDelay)
            {
                inCombo = false;
                comboCounter = 0;
            }
        }
    }

    public virtual void BackToPool()
    {
        gameManager.RemoveEnemy();
        //Implement at Inherited enemy
    }
}
