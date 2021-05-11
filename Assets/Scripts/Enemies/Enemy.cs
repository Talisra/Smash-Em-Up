﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public abstract class Enemy : MonoBehaviour, IPoolableObject
{
    // References
    public GameObject bullet;
    public GameObject player;

    protected AudioManager audioManager;
    protected Rigidbody rb;
    protected Collider[] enemyColliders;
    private ComboManager comboManager;
    private GameManager gameManager;
    public bool tempDisable = false;

    // Graphics
    public GameObject body;
    public GameObject superSpeedKernel;
    public Texture normalTex;
    public Texture flashTex;
    protected Renderer rend;
    protected TrailRenderer tr;

    // Stats
    public float moveSpeed = 10f;
    public int maxHealth;
    protected int curHealth; // number of times the enemy can hit the walls before exploding
    public float flashTime = 4;
    protected float flashCounter = 0;
    protected bool isFlashing = false;
    protected bool isAlive = true;
    protected float minVelocityForDamage = 5f;

    // Squash
    public bool isSquashable;
    public bool isTouchingPlayer = false;
    public bool isSquashed = false;
    public Vector3 squashVector;
    private Vector3 normalScale;

    // Combos
    private bool inCombo = false;
    private float comboCounter = 0;
    private float comboDelay = 2f;

    // SuperSpeed
    private bool isSuperSpeed = false;
    private float superSpeedCounter = 0;
    private float superSpeedDelay;
    private float superSpeedMagnitude = 0;

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
        normalScale = transform.localScale;
        superSpeedKernel.transform.localScale = 
            superSpeedKernel.transform.localScale + (new Vector3(1, 1, 1) - normalScale);
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
        enemyColliders = GetComponentsInChildren<Collider>();
    }

    private void OnEnable()
    {
        if (tempDisable)
        {
            tempDisable = false;
            StartCoroutine(Behavior());
        }
        else
        {
            // fix all the things with delay and counter: ESPECIALLY SUPER SPEED!
            transform.localScale = normalScale;
            isSquashed = false;
            rb.isKinematic = false;
            isSuperSpeed = false;
            superSpeedKernel.SetActive(false);
            isAlive = true;
            foreach (Collider c in enemyColliders)
                c.enabled = true;
            curHealth = maxHealth;
            StartCoroutine(Behavior());
        }
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
                Damage(1);
            }
            else if (collision.gameObject.tag == "Enemy" && isAlive && !isFlashing)
            {
                Damage(1);
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

    public virtual void Squash()
    {
        if (!isSquashed)
        {
            StopAllCoroutines();
            audioManager.Play("Squash");
            isSquashed = true;
            rb.isKinematic = true;
            transform.localScale = squashVector;
            rb.MoveRotation(Quaternion.Euler(0, 0, 0));
            AttachToWall();
            foreach (Collider c in enemyColliders)
                c.enabled = false;
            Damage(curHealth);

        }
    }

    private void AttachToWall()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, new Vector3(Mathf.Sign(rb.velocity.x),0,0), out hit)) 
        { 


        }
        /*
        RaycastHit hit;
        if (Physics.Raycast(transform.position, new Vector3(Mathf.Sign(rb.velocity.x), 0, 0), out hit))
        {
            transform.position = transform.position + new Vector3(hit.transform.position.x * Mathf.Sign(rb.velocity.x), 0 ,0);
        }
        */
    }

    public void Damage(int amount)
    {
        if (isAlive)
        {
            if (inCombo)
            {
                ResetComboChain();
                comboManager.AddCombo();
            }
            audioManager.Play(hitAudio);
            curHealth -= amount;
            Flash();
            if (curHealth <= 0)
            {
                // destroy this object and spawn scraps (visual only)
                isAlive = false;
                if (isSquashed)
                    Invoke("Explode", isSquashable ? Random.Range(0.3f, 1.1f) : 0);
                else
                    Explode();
            }
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
        CameraShake.Shake(0.5f, 0.4f);
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
        StopMovement();
        BackToPool();
    }

    public void GiveSuperSpeed(float duration)
    {
        isSuperSpeed = true;
        superSpeedKernel.SetActive(true);
        superSpeedDelay = duration;
    }

    public void StopMovement()
    {
        rb.velocity = Vector3.zero;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (rb.velocity.magnitude > 20f)
            if (!isSuperSpeed)
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
        if (isSuperSpeed)
        {

            superSpeedCounter += Time.deltaTime;
            if (superSpeedCounter >= superSpeedDelay)
            {
                isSuperSpeed = false;
                superSpeedCounter = 0;
                superSpeedMagnitude = 0;
                superSpeedKernel.SetActive(false);
            }
        }
    }

    private void FixedUpdate()
    {
        if (isSuperSpeed)
        {
            if (superSpeedMagnitude == 0 && rb.velocity.magnitude > 0)
            {
                superSpeedMagnitude = rb.velocity.magnitude;
            }
            if (rb.velocity.magnitude < superSpeedMagnitude)
            {
                rb.velocity = rb.velocity.normalized * superSpeedMagnitude;
            }
        }
    }

    public virtual void BackToPool()
    {
        gameManager.RemoveEnemy();
        //Implement at Inherited enemy
    }
}
