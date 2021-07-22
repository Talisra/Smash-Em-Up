using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public abstract class Enemy : MonoBehaviour, IPoolableObject
{
    // References
    public GameObject bullet;
    public Player player;

    [HideInInspector]
    public Rigidbody rb;
    protected Collider[] enemyColliders;
    private ComboManager comboManager;
    [HideInInspector]
    public bool tempDisable = false;
    [HideInInspector]
    public bool inGame = false;

    // Graphics
    public List<GameObject> body;
    public GameObject superSpeedKernel;
    public Texture normalTex;
    public Texture flashTex;
    protected List<Renderer> rends = new List<Renderer>();
    protected TrailRenderer tr;

    // Stats
    public float moveSpeed = 10f;
    public int maxHealth;
    protected int curHealth; // number of times the enemy can hit the walls before exploding
    private float flashTime = 0.3f;
    protected float flashCounter = 0;
    protected bool isFlashing = false;
    protected bool isAlive = true;
    protected float minVelocityForDamage = 10f;

    // Thrust
    public bool isThrusting;
    public float thrustSpeed;
    protected float thrustCounter = 0;
    private float thrustDelay = 5f;
    private float thrustInvokeDelay = 0.3f;
    private float thrustInvokeCounter = 0f;
    private bool thrustInvokeActive = false;
    private ThrustEffect thrustEffect;

    // Squash
    public bool isSquashable;
    public bool isTouchingPlayer = false;
    public bool isSquashed = false;
    public Vector3 squashVector;
    private Vector3 normalScale;

    // Damaged
    private int lastCollisionID = 0;

    // Hit By Player
    public bool isHit;
    private float hitCounter;
    private float hitDelay = 0.5f;
    public GameObject hitFX;

    // Combos
    private bool inCombo = false;
    private float comboCounter = 0;
    private float comboDelay = 2f;

    // SuperSpeed
    private bool isSuperSpeed = false;
    private float superSpeedCounter = 0;
    private float superSpeedDelay;
    public float superSpeedVelocity = 75;

    // Audio
    public string afterEffectAudio;
    public string explodeAudio;
    public string hitAudio;

    // Destruction
    public List<GameObject> scraps;
    public GameObject explosionPrefab;

    // UI
    private EnemyHPBar hpBar;

    private int[] numOfScraps; // visual: the number of small pieces the enemy will spawn when it dies
    void Awake()
    {
        ChangeLayerRecursive(14);
        normalScale = transform.localScale;
        superSpeedKernel.transform.localScale = 
            superSpeedKernel.transform.localScale + (new Vector3(1, 1, 1) - normalScale);
        numOfScraps = new int[scraps.Count];
        for (int i = 0; i < scraps.Count; i++)
        {
            Scraps temp = scraps[i].GetComponent<Scraps>();
            numOfScraps[i] = temp.GenerateAmount();
        }
        comboManager = FindObjectOfType<ComboManager>();
        player = FindObjectOfType<Player>();
        foreach(GameObject go in body)
        {
            rends.Add(go.GetComponent<Renderer>());
        }
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<TrailRenderer>();
        enemyColliders = GetComponentsInChildren<Collider>();
        GameObject hpgo = Instantiate(Resources.Load("EnemyHPBar", typeof(GameObject)),
            transform.position, Quaternion.identity) as GameObject;
        hpBar = hpgo.GetComponent<EnemyHPBar>();
        hpBar.AttatchToEnemy(this);
        GameObject thrustgo = Instantiate(Resources.Load("ThrustEffect", typeof(GameObject)),
            transform.position, Quaternion.identity) as GameObject;
        thrustEffect = thrustgo.GetComponent<ThrustEffect>();
        thrustEffect.gameObject.SetActive(false);
    }

    protected virtual void OnEnable()
    {
        if (tempDisable)
        {
            tempDisable = false;
            StartCoroutine(Behavior());
        }
        else
        {
            ChangeLayerRecursive(14);
            transform.localScale = normalScale;
            isSquashed = false;
            rb.isKinematic = false;
            isSuperSpeed = false;
            superSpeedKernel.SetActive(false);
            hpBar.gameObject.SetActive(true);
            isAlive = true;
            foreach (Collider c in enemyColliders)
                c.enabled = true;
            curHealth = maxHealth;
            StartCoroutine(Behavior());
        }
    }

    public void SetInGame()
    {
        inGame = true;
        ChangeLayerRecursive(12);
    }

    private void ChangeLayerRecursive(int layer)
    {
        gameObject.layer = layer;
        foreach (Transform child in transform)
            child.gameObject.layer = layer;
    }

    public int GetMaxHp()
    {
        return maxHealth;
    }

    public int GetCurrentHp()
    {
        return curHealth;
    }

    public Rigidbody GetRigidbody()
    {
        return rb;
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

    protected void Thrust(Vector3 target)
    {
        if (!isThrusting)
        {
            isThrusting = true;
            thrustCounter = 0;
            rb.AddForce((target - transform.position).normalized * thrustSpeed,
                ForceMode.VelocityChange);
            thrustEffect.SetEffect(transform.position, target);
        }
    }

    protected Vector3 ThrustLocation()
    {
        int x_offset = Random.Range(-5, 5);
        int y_offset = Random.Range(-5, 5);
        Vector3 target = new Vector3(x_offset, y_offset, 0);
        return target;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //collidingObjects++;
        if (rb.velocity.magnitude > minVelocityForDamage) // damage is only possible when the enemy has some acceleration
        {
            if (isAlive && rb.velocity.magnitude > minVelocityForDamage)
            {
                if (collision.gameObject.GetInstanceID() != lastCollisionID)
                {
                    if (collision.gameObject.tag == "Unpassable" || collision.gameObject.tag == "Enemy")
                    {
                        Damage(isSuperSpeed ? 2 : 1);
                    }
                }
            }
        }
        lastCollisionID = collision.gameObject.GetInstanceID();
    }

    public void ResetComboChain()
    {
        comboCounter = 0;
        inCombo = true;
    }

    public virtual void HitByPlayer()
    {
        AudioManager.Instance.Play(hitAudio);
        isHit = true;
    }

    public virtual void Squash()
    {
        if (!isSquashed)
        {
            StopAllCoroutines();
            AudioManager.Instance.Play("Squash");
            isSquashed = true;
            transform.localScale = squashVector;
            AttachToWall();
            rb.MoveRotation(Quaternion.Euler(0, 0, 0));
            rb.isKinematic = true;
            foreach (Collider c in enemyColliders)
                c.enabled = false;
            Damage(curHealth);
        }
    }
    private void AttachToWall()
    {
        float sign = Mathf.Sign(transform.position.x);
        transform.position = new Vector3(
            (sign < 0 ? GameManager.Instance.GetGameArea()[0] : GameManager.Instance.GetGameArea()[2]) - sign * squashVector.x,
            transform.position.y, 0);
    }

    public void InstantKill()
    {
        if (isAlive)
        {
            isAlive = false;
            curHealth = 0;
            Explode();
        }
    }

    public virtual void Damage(int amount)
    {
        if (isAlive && inGame)
        {
            if (inCombo)
            {
                ResetComboChain();
                comboManager.AddCombo();
            }
            AudioManager.Instance.Play(hitAudio);
            Instantiate(hitFX, transform.position, Quaternion.identity);
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
        flashCounter = 0;
        foreach(Renderer rend in rends)
        {
            rend.material.SetTexture("_MainTex", flashTex);
        }
    }

    protected virtual void BeforeExplode()
    {

    }

    // Handle Destruction when enemy is 0 HP:
    // Create an explosion prefab and scraps.
    public void Explode()
    {
        BeforeExplode();
        CameraEffects.Shake(0.5f, 0.4f);
        AudioManager.Instance.Play(explodeAudio);
        if (afterEffectAudio != "")
            AudioManager.Instance.Play(afterEffectAudio);
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
                GameObject createdScrap = PrefabPooler.Instance.Get(
                    scraps[i].name, transform.position, Quaternion.identity) as GameObject;
                // each scrap has random size and rotation
                Vector3 size = scrap.GetComponent<Scraps>().GenerateSize();
                createdScrap.transform.localScale = size;
                Quaternion rotationTarget = Quaternion.Euler(Random.Range(0, 359), Random.Range(0, 359), Random.Range(0, 359));
                createdScrap.transform.rotation = Quaternion.Slerp(createdScrap.transform.rotation, rotationTarget, 0);
            }
        }
        StopMovement();
        Invoke("BackToPool", 0.1f);
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
        // Check if the enemy went outside the GameArea 
        List<float> gameArea = GameManager.Instance.GetGameArea();
        if (inGame)
        {
            if (transform.position.x < gameArea[0] - 5 || transform.position.x > gameArea[2] + 5 || transform.position.y < gameArea[1] - 5 || transform.position.y > gameArea[3] + 5)
            {
                ChangeLayerRecursive(14);
                inGame = false;
                WaveManager.Instance.RespawnEnemy(this);
            }
        }
        if (rb.velocity.magnitude > minVelocityForDamage)
        {
            if (!isSuperSpeed)
                tr.enabled = true;
            else
                tr.enabled = false;
        }
        else
            tr.enabled = false;
        if (isFlashing)
        {
            flashCounter += Time.deltaTime;
            if (flashCounter >= flashTime)
            {
                foreach(Renderer rend in rends)
                {
                    rend.material.SetTexture("_MainTex", normalTex);
                }
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
        if (isHit)
        {
            hitCounter += Time.deltaTime;
            if (hitCounter >= hitDelay)
            {
                isHit = false;
                hitCounter = 0;
            }
        }
        if (isSuperSpeed)
        {

            superSpeedCounter += Time.deltaTime;
            if (superSpeedCounter >= superSpeedDelay)
            {
                isSuperSpeed = false;
                superSpeedCounter = 0;
                superSpeedKernel.SetActive(false);
            }
        }
        if (thrustInvokeActive)
        {
            thrustInvokeCounter += Time.deltaTime;
        }
        if (thrustInvokeCounter >= thrustInvokeDelay)
        {
            Thrust(ThrustLocation());
            thrustInvokeActive = false;
            thrustInvokeCounter = 0;
        }
        if (isThrusting)
        {
            thrustCounter += Time.deltaTime;
            if (thrustCounter >= thrustDelay)
            {
                isThrusting = false;
                thrustCounter = 0;
            }
        }
    }

    private void FixedUpdate()
    {
        
        if (rb.velocity.magnitude < 5 && rb.velocity.magnitude != 0)
        {
            if (!isThrusting)
            {
                thrustInvokeActive = true;
            }
        }
        if (isSuperSpeed)
        {
            rb.velocity = rb.velocity.normalized * superSpeedVelocity;
            /*
            if (superSpeedMagnitude == 0 && rb.velocity.magnitude > 0)
            {
                superSpeedMagnitude = rb.velocity.magnitude;
            }
            if (rb.velocity.magnitude < superSpeedMagnitude)
            {
                rb.velocity = rb.velocity.normalized * superSpeedMagnitude;
            }*/
        }
    }

    public virtual void BackToPool()
    {
        WaveManager.Instance.RemoveEnemy(this);
        inGame = false;
        hpBar.gameObject.SetActive(false);
        //Implement at Inherited enemy
    }
}
