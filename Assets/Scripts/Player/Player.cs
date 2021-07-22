using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Head head;
    public RegularBody body;
    public PlayerBottom bottom;
    public GameObject animationAnchor;
    public Animator animator;
    public Rigidbody rb;

    public GameObject smashAnimPrefab;
    // Skills
    private Skill[] skills;
    private Skill currentSkill = null; // -1 idle, while any other skill indicates the current skill active.

    // Death
    private bool isDead = false;
    private bool bigExpInvoked = false;
    public GameObject scrapPrefab1;
    public GameObject scrapPrefab2;
    public GameObject explosionPrefabBig;
    public GameObject explosionPrefabSmall;
    private int numOfExps = 9;
    private int numOfScraps = 20;

    // Squash
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
    public float maxHP = 10;
    private float currentHP;

    // Movement
    public float MoveSpeed = 0.1f;
    public float normalRotationCoefficient = 1f;
    public float deltaCap = 10f;
    private float MaxSpeed = 11.0f;
    private float velocity = 3.0f;
    private Vector2 mousePosition;
    private Vector3 position = Vector3.zero;
    private float deltaX;
    private float deltaY;
    private float out_of_screen_mult = 1.35f;
    private float rotationCoefficient;
    private bool inControl = true;

    // Attack
    public int maxPowerUps;
    public float power = 15f;
    public float basePower = 25f;
    private int currentPowerUps = 5;
    private bool canAttack = true;
    private float attackDelay = 0.4f;
    private float atkDelayCounter = 0;

    private int attackSequence = 0;
    private bool inAtkCombo = false;
    private float atkSeqComboTime = 2f;
    private float atkSeqComboCounter = 0;

    //Invulnerability & Shield
    private bool isInvul = false;
    private float invTime = 0.75f;
    private float invDelayCounter = 0;

    private bool isShielded = false;
    private float shieldTime = 0;
    private float shieldDelayCounter = 0;

    // Stun
    private bool isStunned = false;
    private float stunDelay = 0.5f;
    private float stunDelayCounter = 0;

    void Awake()
    {
        //audioManager = FindObjectOfType<AudioManager>();
        animator = animationAnchor.GetComponent<Animator>();
        currentHP = maxHP;
        rotationCoefficient = normalRotationCoefficient;
        rb = GetComponent<Rigidbody>();
    }

    public void AssignSkills(Skill[] skills) // skills[0] is left mouse, while skills[1] is right mouse
    {
        this.skills = skills;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Head Collision
        if (collision.GetContact(0).thisCollider.gameObject.tag == "PlayerHead")
        {
            // Hitting an enemy will add force to the enemy, depends on deltaX.
            // The higher deltaX, more force added.
            
            if (collision.gameObject.tag == "Enemy")
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                if (!enemy.isSquashed)
                {
                    GainInv(0.3f);
                    if (currentSkill != null)
                    {
                        SkillSmash(enemy);
                    }
                    else if (canAttack)
                        BasicSmash(enemy);
                }
            }
            // Bullet that hitting the Head will do no damage
            else if(collision.gameObject.tag == "Bullet")
            {
                //collision.gameObject.GetComponent<BasicBullet>().Explode();
            }
            else if(collision.gameObject.tag == "Miniboss")
            {
                if (Mathf.Abs(rb.velocity.x) > 0 && canAttack && currentSkill==null)
                {
                    Miniboss miniboss = collision.gameObject.GetComponentInParent<Miniboss>();
                    miniboss.TakeDamage(1, collision.GetContact(0).point);
                    BasicSmash(collision.GetContact(0).point);
                }
                else if (currentSkill != null)
                {
                    Miniboss miniboss = collision.gameObject.GetComponentInParent<Miniboss>();
                    if (miniboss.canTakeDmg)
                    {
                        currentSkill.OnSmashVoid(collision.GetContact(0).point);
                        miniboss.TakeDamage(currentSkill.minibossDmg, collision.GetContact(0).point);
                    }

                }
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
                {
                    TakeDamage(1, true, true, collision.transform.position);
                }
            }
            else if (collision.gameObject.tag == "Bullet")
            {
                TakeDamage(1, true, true, collision.transform.position);
                //collision.gameObject.GetComponent<BasicBullet>().Explode();
            }
        }
        // General Collisions
        if (collision.gameObject.tag == "Unpassable")
        {
            if (currentSkill != null)
            {
                currentSkill.OnWallCollision(collision);
            }
        }
        if(collision.gameObject.tag == "MapBonus")
        {
            if (currentSkill != null)
            {
                MapBonus mb = collision.gameObject.GetComponentInParent<MapBonus>();
                if (mb != null)
                    currentSkill.OnMapBonusCollision(mb);
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
    public float GetDeltaX()
    {
        return deltaX;
    }

    public float GetDeltaY()
    {
        return deltaY;
    }

    public void Heal(int amount)
    {
        if (isDead)
            return;
        if (currentHP + amount >= maxHP)
            currentHP = maxHP;
        else
            currentHP += amount;
    }
    public void Squash(GameObject colliderObj)
    {
        if (!isSquashed)
        {
            AudioManager.Instance.Play("Squash");
            isSquashed = true;
            TakeDamage(1, false, false, Vector3.zero);
            MoveSpeed = 0.9f;
            transform.localScale = squashScaleVector;
            colliderObj.GetComponent<Rigidbody>().AddForce(new Vector3(colliderObj.transform.position.x - transform.position.x,20,0), ForceMode.VelocityChange);
        }
    }

    public void GiveControl()
    {
        inControl = true;
    }

    public void TakeControl()
    {
        inControl = false;
        this.rb.velocity = Vector3.zero;
    }

    public void Stun(float delay)
    {
        isStunned = true;
        stunDelayCounter = 0;
        stunDelay = delay;
    }

    public void GainShield(float amount) // same like GainInv but also gives the shieldFX
    {
        isShielded = true;
        shieldDelayCounter = 0;
        shieldTime = amount;
        GameObject shield = PrefabPooler.Instance.Get(
            "Shield", transform.position, Quaternion.identity) as GameObject;
        shield.GetComponent<ShieldFX>().SetShield(gameObject, amount);
    }

    public void GainInv(float amount)
    {
        isInvul = true;
        invDelayCounter = 0;
        invTime = amount;
    }

    public void CancelInv() //TODO: currently, cancel inv will not affect shieldFX!
    {
        invDelayCounter = invTime;
    }

    public void TakeDamage(float amount, bool giveShield, bool showHit,Vector3 hitCoordinates)
    {
        if (!isInvul && !isShielded)
        {
            GameManager.Instance.FlashRedScreen();
            body.ShowDamage();
            bottom.ShowDamage();
            currentHP -= amount;
            if (currentHP <= 0)
            {
                Die();
                return;
            }
            if (giveShield)
                GainShield(0.75f);
            if (showHit)
                BleedExplosion(hitCoordinates);
        }
    }

    public void TakeTrueDamage(float amount, bool giveShield, bool showHit, Vector3 hitCoordinates) // use when the player should take damage regardless invulnerability
    {
        if (!isShielded)
        {
            GameManager.Instance.FlashRedScreen();
            body.ShowDamage();
            bottom.ShowDamage();
            currentHP -= amount;
            if (currentHP <= 0)
            {
                Die();
                return;
            }
            if (giveShield)
                GainShield(0.75f);
            if (showHit)
                BleedExplosion(hitCoordinates);
        }
    }

    private void Die()
    {
        if (!isDead)
        {
            isDead = true;
            Invoke("CreateBigExplosion", Random.Range(0.6f, 0.7f));
            for (int i = 0; i < numOfExps; i++)
            {
                Invoke("CreateTinyExplosion", Random.Range(0.1f, 0.7f));
            }
            Destroy(gameObject, 0.91f);
        }
    }
    private void CreateBigExplosion()
    {
        GameManager.Instance.EndGame();
        bigExpInvoked = true;
        CameraEffects.Shake(2.5f, 1.3f);
        AudioManager.Instance.Play("PlayerExplosion");
        GameObject bigExp = Instantiate(
            explosionPrefabBig, transform.position, Quaternion.identity) as GameObject;
        Destroy(bigExp, bigExp.GetComponent<ParticleSystem>().main.duration);
        for (int j = 0; j < numOfScraps; j++)
        {
            GameObject createdScrap = 
                PrefabPooler.Instance.Get(scrapPrefab1.name, transform.position, Quaternion.identity) as GameObject;
            // each scrap has random size and rotation
            float size = Random.Range(0.1f, 0.5f);
            createdScrap.transform.localScale = new Vector3(size, size, size);
            Quaternion rotationTarget = Quaternion.Euler(Random.Range(0, 359), Random.Range(0, 359), Random.Range(0, 359));
            createdScrap.transform.rotation = Quaternion.Slerp(createdScrap.transform.rotation, rotationTarget, 0);
        }
        for (int j = 0; j < numOfScraps; j++)
        {
            GameObject createdScrap = 
                PrefabPooler.Instance.Get(scrapPrefab2.name, transform.position, Quaternion.identity) as GameObject;
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
        if (!bigExpInvoked)
            CameraEffects.Shake(0.2f, 0.35f);
        AudioManager.Instance.Play("SmallExplosion");
        GameObject smallExp = Instantiate(
                explosionPrefabSmall, 
                new Vector3(
                    transform.position.x + Random.Range(-1,1),
                    transform.position.y + Random.Range(-1,1),
                    transform.position.z + Random.Range(-1,1)),
                Quaternion.identity) as GameObject;
        Destroy(smallExp, smallExp.GetComponent<ParticleSystem>().main.duration);
    }

    private void BleedExplosion(Vector3 position)
    {
        AudioManager.Instance.Play("SmallExplosion");
        GameObject smallExp = Instantiate(
                explosionPrefabSmall, position, Quaternion.identity) as GameObject;
        Destroy(smallExp, smallExp.GetComponent<ParticleSystem>().main.duration);
    }

    private void PerformSkill(Skill skill)
    {
        AddPowerUp(-skill.cost); // reduce the powerup cost
        currentSkill = skill;
        GainInv(skill.invTime);
        if (skill.takeControl)
            TakeControl();
        skill.OnStartAction();
    }

    public bool canUseSkill(Skill skill)
    {
        if (currentPowerUps >= skill.cost && currentSkill == null)
            return true;
        return false;
    }

    public void BackToIdle()
    {
        if (currentSkill)
        {
            currentSkill.OnEndAction();
            currentSkill = null;
        }
    }

    public void SkillSmash(Enemy enemy)
    {
        enemy.ResetComboChain();
        currentSkill.OnSmash(enemy);
    }

    public void BasicSmash(Enemy enemy) 
    {
        inAtkCombo = true;
        atkSeqComboCounter = 0;
        string animationLeft = "";
        string animationRight = "";
        Vector3 powerVector = Vector3.zero;
        enemy.ResetComboChain();
        enemy.HitByPlayer();
        canAttack = false;
        float enemyDirection = transform.position.x - enemy.transform.position.x; // negative = left, positive = right
        Rigidbody rbenemy = enemy.GetComponent<Rigidbody>();
        switch (attackSequence)
        {
            case 0:
                {
                    CameraEffects.Shake(0.1f, 0.2f);
                    AudioManager.Instance.Play("Smash1");
                    powerVector = new Vector3(
                        Mathf.Sign(enemyDirection) * basePower + Mathf.Log((deltaX < 1 ? 1 : deltaX)) * power,
                        0, 0);
                    animationLeft = "AtkLeft";
                    animationRight = "AtkRight";
                    ShowSmashParticle(enemy.transform.position, 2f);
                    attackSequence++;
                    break;
                }
            case 1:
                {
                    CameraEffects.Shake(0.15f, 0.3f);
                    AudioManager.Instance.Play("Smash2");
                    powerVector = new Vector3(
                        Mathf.Sign(enemyDirection) * basePower*2f + Mathf.Log((deltaX < 1 ? 1 : deltaX)) * power*1.5f,
                        0, 0);
                    animationLeft = "AtkLeft2";
                    animationRight = "AtkRight2";
                    ShowSmashParticle(enemy.transform.position, 2.5f);
                    attackSequence++;
                    break;
                }
            case 2: // == maxAtkSequence!
                {
                    enemy.Damage(1);
                    enemy.GiveSuperSpeed(0.5f);
                    CameraEffects.Shake(0.4f, 0.4f);
                    AudioManager.Instance.Play("Smash3");
                    AudioManager.Instance.Play("SmashF");
                    powerVector = new Vector3(
                        Mathf.Sign(enemyDirection) * basePower*5f + Mathf.Log((deltaX < 1 ? 1 : deltaX)) * power*2f,
                        0, 0);
                    animationLeft = "AtkLeftF";
                    animationRight = "AtkRightF";
                    attackSequence = 0;
                    ShowSmashParticle(enemy.transform.position, 3f);
                    break;
                }
        }
        if (enemyDirection > 0) // play the right attack animation
            animator.Play(animationLeft);
        else if (enemyDirection < 0)
            animator.Play(animationRight);
        rbenemy.AddForce(powerVector, ForceMode.VelocityChange);
    }

    // calling BasicSmash with a transform will only apply the animation
    private void BasicSmash(Vector3 target)
    {
        inAtkCombo = true;
        atkSeqComboCounter = 0;
        string animationLeft = "";
        string animationRight = "";
        Vector3 powerVector = Vector3.zero;
        canAttack = false;
        switch (attackSequence)
        {
            case 0:
                {
                    CameraEffects.Shake(0.1f, 0.2f);
                    AudioManager.Instance.Play("Smash1");
                    animationLeft = "AtkLeft";
                    animationRight = "AtkRight";
                    ShowSmashParticle(target, 2f);
                    attackSequence++;
                    break;
                }
            case 1:
                {
                    CameraEffects.Shake(0.15f, 0.3f);
                    AudioManager.Instance.Play("Smash2");
                    animationLeft = "AtkLeft2";
                    animationRight = "AtkRight2";
                    ShowSmashParticle(target, 2.5f);
                    attackSequence++;
                    break;
                }
            case 2: // == maxAtkSequence!
                {
                    CameraEffects.Shake(0.4f, 0.4f);
                    AudioManager.Instance.Play("Smash3");
                    AudioManager.Instance.Play("SmashF");
                    animationLeft = "AtkLeftF";
                    animationRight = "AtkRightF";
                    attackSequence = 0;
                    ShowSmashParticle(target, 3f);
                    break;
                }
        }
        if (target.x > 0) // play the right attack animation
            animator.Play(animationLeft);
        else if (target.x < 0)
            animator.Play(animationRight);
    }

    private void ShowSmashParticle(Vector3 pos, float scale)
    {
        GameObject particle = Instantiate(
            smashAnimPrefab, pos, Quaternion.identity) as GameObject;
        particle.transform.localScale = new Vector3(scale, scale, scale);
        ParticleSystem parts = particle.GetComponent<ParticleSystem>();
        Destroy(particle, parts.main.duration);
    }

    private void TranslateCursorCoordinates()
    {
        mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(
            new Vector3(mousePosition.x, mousePosition.y, -Camera.main.transform.position.z));
        mousePosition.x =
            (mousePosition.x < GameManager.Instance.GetGameArea()[0] ||
            mousePosition.x > GameManager.Instance.GetGameArea()[2]) ? 
            mousePosition.x * out_of_screen_mult : mousePosition.x;
        position = Vector3.Lerp(transform.position, mousePosition, MoveSpeed);
    }

    private void ManageSkillInput(int mouseButton, int skillIndex)
    {
        if (Input.GetMouseButtonDown(mouseButton))
        {
            if (canUseSkill(skills[skillIndex]))
                PerformSkill(skills[skillIndex]);
        }
        if (Input.GetMouseButtonUp(mouseButton))
        {
            if (currentSkill)
                if (currentSkill.isCharge)
                {
                    currentSkill.OnInputRelease();
                }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSquashed && inControl)
        {
            // Special attack #1
            ManageSkillInput(0, 0);
            ManageSkillInput(1, 1);
                     
            //handle attack speed
            if (canAttack == false)
            {
                atkDelayCounter += Time.deltaTime;
                if (atkDelayCounter >= attackDelay)
                {
                    atkDelayCounter = 0;
                    canAttack = true;
                }
            }
        }

        if (inAtkCombo)
        {
            atkSeqComboCounter += Time.deltaTime;
            if (atkSeqComboCounter >= atkSeqComboTime)
            {
                attackSequence = 0;
                atkSeqComboCounter = 0;
                inAtkCombo = false;
            }
        }


        //handle Stun
        if (isStunned)
        {
            stunDelayCounter += Time.deltaTime;
            if (stunDelayCounter >= stunDelay)
            {
                isStunned = false;
                stunDelayCounter = 0;
            }
        }

        //handle Invulnerability
        if (isInvul)
        {
            invDelayCounter += Time.deltaTime;
            if (invDelayCounter >= invTime)
            {
                isInvul = false;
                invDelayCounter = 0;
            }
        }

        //handle Shield
        if (isShielded)
        {
            shieldDelayCounter += Time.deltaTime;
            if (shieldDelayCounter >= shieldTime)
            {
                isShielded = false;
                shieldDelayCounter = 0;
            }
        }

        //handle Squash
        if (isSquashed)
        {
            MoveSpeed -= Time.deltaTime / 5;
            squashCounter += Time.deltaTime;

            //squashScaleVector is Vector3(2, 0.15f, 1.1f);
            transform.localScale += new Vector3(
                    -(squashScaleVector.x - 1) / squashTime * Time.deltaTime,
                    (1 - squashScaleVector.y) / squashTime * Time.deltaTime,
                    (squashScaleVector.z - 1) / squashTime * Time.deltaTime
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
        // Movement 
        TranslateCursorCoordinates();
        deltaX = Mathf.Clamp(position.x - mousePosition.x, -deltaCap, deltaCap);
        deltaY = (position.y - mousePosition.y) * (isSquashed ? squashCounter / squashTime : 1f);

        // lets the player "follow" the mouse
        if (!isStunned && inControl)
        {
            float xForce = velocity * -deltaX - rb.velocity.x;
            xForce = Mathf.Clamp(xForce, -MaxSpeed, MaxSpeed);
            float yForce = velocity * -deltaY - rb.velocity.y;
            rb.AddForce(new Vector3(xForce, yForce, 0), ForceMode.VelocityChange);
            Quaternion rotationTarget = Quaternion.Euler(0, 0,
            Mathf.Clamp(
                deltaX * rotationCoefficient,
                -90, 90));
            rb.MoveRotation(rotationTarget);
        }
    }
}
