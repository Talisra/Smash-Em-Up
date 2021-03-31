using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Head head;
    public GameObject body;
    public GameObject bottom;
    public GameObject animationAnchor;

    private Animator animator;
    private Rigidbody rb;
    private AudioManager audioManager;

    public GameObject smashAnimPrefab;

    // Death
    public GameObject scrapPrefab1;
    public GameObject scrapPrefab2;
    public GameObject explosionPrefabBig;
    public GameObject explosionPrefabSmall;
    private int numOfExps = 9;
    private int numOfScraps = 20;

    // Sqash
    private Vector3 squashScaleVector = new Vector3(2, 0.15f, 1.1f);
    private float squashTime = 1.0f;
    private float squashCounter = 0;
    private bool isSquashed = false;

    public bool collidingFloor = false;
    public List<int> collidingBoxesID = new List<int>();

    // the enemies will target these points to do damage
    // top and bottom points depend on the y axis
    public Transform WeakPointTop;
    public Transform WeakPointBot;


    // Attributes
    private float maxHP = 10;
    private float currentHP;

    // After taking damage, gain Invulnerability for some time
    private float invTime = 0.5f;
    private float invCounter = 0;
    private bool isInvul = false;


    // Movement
    public float MoveSpeed = 0.1f;
    public float smoothTime = 0.1f; // lower smoothTime means better control with the mouse.
    private float velocity = 3f;
    private Vector3 mousePosition;
    private Vector3 position = Vector3.zero;
    private float deltaX;
    private float deltaY;
    private float rotationCoefficient = 0.9f;

    // Attack
    public int maxPowerUps;
    public float power = 15f;
    public float basePower = 25f;
    private int currentPowerUps = 5;
    //private float specialModifier = 7000;
    private bool canAttack = true;
    private bool isAttackingSpecial = false;
    private float attackDelay = 0.4f;
    private float delay = 0;


    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        animator = animationAnchor.GetComponent<Animator>();
        currentHP = maxHP;
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Head Collision
        if (collision.contacts[0].thisCollider.gameObject.tag == "PlayerHead")
        {
            // Hitting an enemy will add force to the enemy, depends on deltaX.
            // The higher deltaX, more force added.
            if (collision.gameObject.tag == "Enemy")
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                enemy.isTouchingPlayer = true;
                if (!enemy.isSquashed)
                {
                    if (isAttackingSpecial && canAttack)
                        SpecialSmash(collision.gameObject);
                    else if (canAttack)
                        Smash(collision.gameObject);
                }
            }
            // Bullet that hitting the Head will do no damage
            else if(collision.gameObject.tag == "Bullet")
            {
                collision.gameObject.GetComponent<BasicBullet>().Explode();
            }
        }
        // Body Collision
        else if (collision.contacts[0].thisCollider.gameObject.tag == "PlayerBody")
        {
            // Enemy hitting the body will damage the player
            if (collision.gameObject.tag == "Enemy")
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                if (!enemy.isSquashed)
                    TakeDamage();
            }
            else if (collision.gameObject.tag == "Bullet")
            {
                TakeDamage();
                collision.gameObject.GetComponent<BasicBullet>().Explode();
            }
        }

        if (collision.gameObject.tag == "Floor")
        {
            collidingFloor = true;
        }
        else if (collision.gameObject.tag == "EnemyBox")
        {
            if (!collidingBoxesID.Contains(collision.gameObject.GetInstanceID()))
                collidingBoxesID.Add(collision.gameObject.GetInstanceID());
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.isTouchingPlayer = false;
        }
        // Toggle OFF floor/box collision
        if (collision.gameObject.tag == "Floor")
        {
            collidingFloor = false;
        }
        if (collision.gameObject.tag == "EnemyBox")
        {
            if (collidingBoxesID.Contains(collision.gameObject.GetInstanceID()))
                collidingBoxesID.Remove(collision.gameObject.GetInstanceID());
        }
    }

    public void AddPowerUp(int amount)
    {
        currentPowerUps += amount;
    }

    public int GetCurrentPowerUps()
    {
        return currentPowerUps;
    }

    public Transform GetWeakPointTop()
    {
        return WeakPointTop;
    }    
    
    public Transform GetWeakPointBot()
    {
        return WeakPointBot;
    }

    public float GetMaxHp()
    {
        return maxHP;
    }

    public float GetCurrentHp()
    {
        return currentHP;
    }

    public void Squash(GameObject colliderObj)
    {
        if (!isSquashed)
        {
            audioManager.Play("Squash");
            isSquashed = true;
            TakeDamage();
            MoveSpeed = 0.9f;
            transform.localScale = squashScaleVector;
            colliderObj.GetComponent<Rigidbody>().AddForce(new Vector3(colliderObj.transform.position.x - transform.position.x,20,0), ForceMode.VelocityChange);
        }
    }


    public void TakeDamage()
    {
        if (!isInvul)
        {
            FindObjectOfType<HitFlash>().FlashDamage();
            currentHP--;
            if (currentHP <= 0)
                Die();
            isInvul = true;
        }
    }

    private void Die()
    {
        Invoke("CreateBigExplosion", Random.Range(0.5f, 0.6f));
        for (int i = 0; i < numOfExps; i++)
        {
            Invoke("CreateTinyExplosion", Random.Range(0.1f, 0.9f));
        }
        FindObjectOfType<GameManager>().EndGame();
        Destroy(gameObject, 0.91f);
    }
    private void CreateBigExplosion()
    {
        CameraShake.Shake(0.7f, 1.2f);
        FindObjectOfType<AudioManager>().Play("PlayerExplosion");
        GameObject bigExp = Instantiate(
            explosionPrefabBig, transform.position, Quaternion.identity) as GameObject;
        Destroy(bigExp, bigExp.GetComponent<ParticleSystem>().main.duration);
        for (int j = 0; j < numOfScraps; j++)
        {
            GameObject createdScrap = 
                ScrapsPooler.Instance.Get(scrapPrefab1.name, transform.position, Quaternion.identity) as GameObject;
            // each scrap has random size and rotation
            float size = Random.Range(0.1f, 0.5f);
            createdScrap.transform.localScale = new Vector3(size, size, size);
            Quaternion rotationTarget = Quaternion.Euler(Random.Range(0, 359), Random.Range(0, 359), Random.Range(0, 359));
            createdScrap.transform.rotation = Quaternion.Slerp(createdScrap.transform.rotation, rotationTarget, 0);
        }
        for (int j = 0; j < numOfScraps; j++)
        {
            GameObject createdScrap = 
                ScrapsPooler.Instance.Get(scrapPrefab2.name, transform.position, Quaternion.identity) as GameObject;
            // each scrap has random size and rotation
            float size = Random.Range(0.1f, 0.5f);
            createdScrap.transform.localScale = new Vector3(size, size, size);
            Quaternion rotationTarget = Quaternion.Euler(Random.Range(0, 359), Random.Range(0, 359), Random.Range(0, 359));
            createdScrap.transform.rotation = Quaternion.Slerp(createdScrap.transform.rotation, rotationTarget, 0);
        }
        gameObject.SetActive(false);
    }

    private void CreateTinyExplosion()
    {
        CameraShake.Shake(0.2f, 0.35f);
        FindObjectOfType<AudioManager>().Play("SmallExplosion");
        GameObject smallExp = Instantiate(
                explosionPrefabSmall, 
                new Vector3(
                    transform.position.x + Random.Range(-1,1),
                    transform.position.y + Random.Range(-1,1),
                    transform.position.z + Random.Range(-1,1)),
                Quaternion.identity) as GameObject;
        Destroy(smallExp, smallExp.GetComponent<ParticleSystem>().main.duration);
    }

    public void SpecialAttack()
    {
        currentPowerUps--; //use powerup
        isInvul = true;
        isAttackingSpecial = true;
        if (deltaX < 0)
            animator.Play("SpAtkRight");
        else
            animator.Play("SpAtkLeft");
        head.ManageAttack(0.25f);
        Invoke("EndSpecialAttack", 0.25f);
    }
    public void EndSpecialAttack()
    {
        isAttackingSpecial = false;
    }

    // basic function of smashing an enemy
    private void SmashActions(GameObject enemy)
    {
        Enemy target = enemy.GetComponent<Enemy>();
        target.ResetComboChain();
        target.HitByPlayer();
        canAttack = false;
    }

    public void SpecialSmash(GameObject enemy)
    {
        SmashActions(enemy);
        CameraShake.Shake(0.25f, 0.3f);
        audioManager.Play("SuperSmash");
        // calculate the distance to know if animation is from left or right
        Vector3 enemyPos = enemy.transform.position;
        Vector3 playerPos = transform.position;
        float enemyDirection = -playerPos.x * enemyPos.y + playerPos.y * enemyPos.x; // negative = left, positive = right
        ShowSmashParticle(enemy.transform);
        Rigidbody rbenemy = enemy.GetComponent<Rigidbody>();
        Vector3 PowerVector = new Vector3(
            Mathf.Sign(enemyDirection) * 7000,
            0, 0);
        rbenemy.AddForce(PowerVector);
    }

    public void Smash(GameObject enemy) 
    {
        SmashActions(enemy);
        CameraShake.Shake(0.1f, 0.2f);
        audioManager.Play("Smash1");
        // calculate the distance to know if animation is from left or right
        Vector3 enemyPos = enemy.transform.position;
        Vector3 playerPos = transform.position;
        float enemyDirection = -playerPos.x * enemyPos.y + playerPos.y * enemyPos.x; // negative = left, positive = right
        if (enemyDirection > 0) // play the right attack animation
            animator.Play("AtkRight");
        else if (enemyDirection < 0)
            animator.Play("AtkLeft");
        ShowSmashParticle(enemy.transform);
        Rigidbody rbenemy = enemy.GetComponent<Rigidbody>();
        Vector3 PowerVector = new Vector3(
            Mathf.Sign(enemyDirection) * basePower + Mathf.Log((deltaX < 1 ? 1 : deltaX)) * power,
            0, 0);
        rbenemy.AddForce(PowerVector);
    }

    private void ShowSmashParticle(Transform pos)
    {
        GameObject particle = Instantiate(
            smashAnimPrefab, pos.position, Quaternion.identity) as GameObject;
        ParticleSystem parts = particle.GetComponent<ParticleSystem>();
        Destroy(particle, parts.main.duration);
    }

    // Update is called once per frame
    void Update()
    {
        // Movement
        mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(
            new Vector3(mousePosition.x, mousePosition.y, -Camera.main.transform.position.z));
        position = Vector3.Lerp(transform.position, mousePosition, MoveSpeed);
        deltaX = (position.x - mousePosition.x);
        deltaY = (position.y - mousePosition.y) * (isSquashed ? squashCounter/squashTime : 1f);      

        if (!isSquashed)
        {
            // Special attack
            if (Input.GetMouseButtonDown(1))
                if (currentPowerUps > 0)
                    SpecialAttack();
           
            //handle attack speed
            if (canAttack == false)
            {
                delay += Time.deltaTime;
                if (delay >= attackDelay)
                {
                    delay = 0;
                    canAttack = true;
                }
            }
        }

        //handle Invulnerability
        if (isInvul)
        {
            invCounter += Time.deltaTime;
            if (invCounter >= invTime)
            {
                isInvul = false;
                invCounter = 0;
            }

        }
        //handle Squash
        if (isSquashed)
        {
            MoveSpeed -= Time.deltaTime/5;
            squashCounter += Time.deltaTime;

            //squashScaleVector is Vector3(2, 0.15f, 1.1f);
            transform.localScale += new Vector3(
                    -(squashScaleVector.x - 1)/ squashTime * Time.deltaTime,
                    (1 - squashScaleVector.y)/ squashTime * Time.deltaTime,
                    (squashScaleVector.z - 1)/ squashTime * Time.deltaTime
                );
            if (squashCounter >= squashTime)
            {
                isSquashed = false;
                squashCounter = 0;
                MoveSpeed = 0.1f;
                transform.localScale = new Vector3(1, 1, 1);
            }

        }
    }
    private void FixedUpdate()
    {
        rb.AddForce(new Vector3(-deltaX* velocity, -deltaY* velocity, 0) - rb.velocity, ForceMode.VelocityChange);
        // tilt the player based on deltaX, only visual, but done through rigidBody to not ruin the physics
        Quaternion rotationTarget = Quaternion.Euler(0, 0, deltaX * rotationCoefficient);
        rb.MoveRotation(rotationTarget);
    }

}
