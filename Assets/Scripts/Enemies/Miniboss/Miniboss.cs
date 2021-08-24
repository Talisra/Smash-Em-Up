using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miniboss : MonoBehaviour
{
    private FanFloor minibossFloor;
    // Model
    public GameObject baseBody;
    public GameObject aimBody;
    public GameObject cannon;
    public GameObject firePoint;

    public Animator rotorAnimator;
    public Animator shockwaveAnimator;

    public List<Renderer> allRenderers;
    public Texture flashTex;
    private List<Texture> normalTextures = new List<Texture>();

    private int currentExpGiven;

    // Shockwave
    private bool disableShockwave = false;
    public GameObject shockwaveKernel;
    public GameObject shockwaveParticle;
    public GameObject shockwaveCircle;
    private float shockwaveDelay = 12;
    private float shockwaveCounter = 0;

    // Aiming and fire
    private Player player;
    private float bulletSpeed = 100;

    private float maxAimSpeed = 10;
    private float minAimSpeed = 0;
    private float currentAimSpeed = 0;
    private float rotationSpeed = 200;
    private bool isRotating = true; // rotor aim

    private float maxFireRate = 2;
    private float minFireRate = 0f;
    private float fireRateInterval = 0.1f;
    private float currentFireRate;
    private float cooldown = 0;

    // Health and damage
    [HideInInspector]
    public int totalHP = 10;
    [HideInInspector]
    public int currentHP;
    public bool canTakeDmg = true;
    private float dmgTakenDelay = 0.35f;
    private float dmgTakenCounter = 0;
    public GameObject takeDmgParticle;
    public GameObject spark;
    public List<GameObject> sparks;
    private int numOfSparks;
    private int sparkIndex;

    // Death/Destruction
    public GameObject bigExp;
    public GameObject bigExp2;
    public GameObject smallExp;
    private bool isDead = false;
    public List<GameObject> scraps;
    private int[] numOfScraps;

    // UI
    private MinibossUI ui;

    private void Start()
    {
        minibossFloor = FindObjectOfType<FanFloor>();
        player = FindObjectOfType<Player>();
        for(int i=0; i<allRenderers.Count; i++)
        {
            normalTextures.Add(allRenderers[i].material.mainTexture);
        }
        numOfScraps = new int[scraps.Count];
        for (int i = 0; i < scraps.Count; i++)
        {
            Scraps temp = scraps[i].GetComponent<Scraps>();
            numOfScraps[i] = temp.GenerateAmount();
        }
        sparks = new List<GameObject>();
        for(int i=0; i<11; i++) // 12 is the maximum sparks available
        {
            sparks.Add(Instantiate(spark, transform.position, Quaternion.identity));
            sparks[i].gameObject.SetActive(false);
        }
        currentFireRate = maxFireRate;
        currentHP = totalHP;
    }

    public void TakeDamage(int amount, Vector3 dmgPosition)
    {
        if (canTakeDmg)
        {
            GameObject damageEffect = Instantiate(
                takeDmgParticle, dmgPosition, Quaternion.identity) as GameObject;
            Destroy(damageEffect, 1f);
            AudioManager.Instance.Play("MinibossDMG");
            currentHP -= amount;
            canTakeDmg = false;
            Flash();
            if ((float)sparkIndex/ (float)numOfSparks < (float)currentHP / (float)totalHP)
            {
                BleedSpark();
            }
            if (currentHP <= 0)
            {
                currentHP = 0;
                Die();
            }
        }
    }

    private void BleedSpark()
    {
        sparks[sparkIndex].SetActive(true);
        sparks[sparkIndex].transform.position = new Vector3(
                    transform.position.x + Random.Range(-3, 3),
                    transform.position.y + Random.Range(-1, 5),
                    transform.position.z + Random.Range(-3, 0));
        sparkIndex++;
    }

    private void Flash()
    {
        foreach(Renderer renderer in allRenderers)
        {
            renderer.material.SetTexture("_MainTex", flashTex);
        }
    }

    private void Unflash()
    {
        for(int i=0; i< allRenderers.Count; i++)
        {
            allRenderers[i].material.SetTexture("_MainTex", normalTextures[i]);
        }
    }

    private void Die()
    {
        if (!isDead)
        {
            isDead = true;
            GameProfile.Instance.AddExpToPool(currentExpGiven);
            AudioManager.Instance.Play("MinibossDie1");
            GameObject bigExpParticle = Instantiate(
                bigExp2, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z - 5),
                Quaternion.identity) as GameObject;
            bigExpParticle.transform.localScale = new Vector3(8f, 8f, 8f);
            Destroy(bigExpParticle, bigExpParticle.GetComponent<ParticleSystem>().main.duration);
            int numOfExp = Random.Range(10, 30);
            for (int i = 0; i < numOfExp; i++)
            {
                Invoke("ExplodeSmall", (2 / numOfExp) * i);
            }
            Invoke("ExplodeBig", 1.9f);
            minibossFloor.Invoke("KillMiniboss", 2f);
            Invoke("DisableUI", 3f);
        }
    }

    private void ExplodeBig()
    {
        foreach(GameObject spark in sparks)
        {
            spark.SetActive(false);
        }
        ExplodeScraps();
        CameraEffects.Shake(0.7f, 1.5f);
        AudioManager.Instance.Play("MinibossDie2");
        AudioManager.Instance.Play("Explosion2");
        GameObject bigExpParticle = Instantiate(
            bigExp, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z),
            Quaternion.identity) as GameObject;
        bigExpParticle.transform.localScale = new Vector3(5.5f, 5.5f, 5.5f);
        Destroy(bigExpParticle, bigExpParticle.GetComponent<ParticleSystem>().main.duration);
    }

    private void ExplodeSmall()
    {
        AudioManager.Instance.Play("Explosion1");
        GameObject smallExpParticle = Instantiate(
                smallExp,
                new Vector3(
                    transform.position.x + Random.Range(-5, 5),
                    transform.position.y + Random.Range(-5, 5),
                    transform.position.z + Random.Range(-3, -6)),
                Quaternion.identity) as GameObject;
        Destroy(smallExpParticle, smallExpParticle.GetComponent<ParticleSystem>().main.duration);
    }

    private void ExplodeScraps()
    {
        for (int i = 0; i < scraps.Count; i++)
        {
            for (int j = 0; j < numOfScraps[i]; j++)
            {
                GameObject scrap = scraps[i];
                GameObject createdScrap = PrefabPooler.Instance.Get(
                    scraps[i].name, 
                    new Vector3(transform.position.x, transform.position.y + 12, transform.position.z),
                    Quaternion.identity) as GameObject;
                // each scrap has random size and rotation
                Vector3 size = scrap.GetComponent<Scraps>().GenerateSize();
                createdScrap.transform.localScale = size;
                Quaternion rotationTarget = Quaternion.Euler(Random.Range(0, 359), Random.Range(0, 359), Random.Range(0, 359));
                createdScrap.transform.rotation = Quaternion.Slerp(createdScrap.transform.rotation, rotationTarget, 0);
            }
        }
    }

    private void DisableUI()
    {
        ui.DisableUI();
    }

    private void Fire(Vector3 target)
    {
        AudioManager.Instance.Play("MiniBossFire");
        rotorAnimator.Play("MinibossShoot");
        MinibossBullet bullet = MinibossBullet_Pool.Instance.Get(firePoint.transform.position, cannon.transform.rotation);
        bullet.Shoot(firePoint.transform.position - cannon.transform.position, bulletSpeed);

    }

    private void ChargeShockwave()
    {
        AudioManager.Instance.Play("MinibossCharge");
        shockwaveKernel.SetActive(true);
        shockwaveAnimator.Play("windup");
        Invoke("ChargeShockwave2", 1.6f);
    }

    private void ChargeShockwave2()
    {
        AudioManager.Instance.Play("MinibossRelease1");
        AudioManager.Instance.Play("MinibossChargeSpark");
        GameObject swp = Instantiate(
        shockwaveParticle, transform.position, Quaternion.identity) as GameObject;
        Destroy(swp, 2f);
        Invoke("ReleaseShockwave", 0.4f);
    }

    private void ReleaseShockwave()
    {
        if (player.transform.position.y <= 0)
        {
            player.TakeDamage(3, true, true, player.transform.position);
        }
        AudioManager.Instance.Play("Cannon_Dub");
        AudioManager.Instance.Play("MinibossRelease2");
        CameraEffects.Shake(0.5f, 1f);
        shockwaveKernel.SetActive(false);
        GameObject swc = Instantiate(
                shockwaveCircle, transform.position, Quaternion.identity) as GameObject;
        Destroy(swc, 1f);
    }

    public void SetDifficulty(int rematch) // each rematch will make the boss stronger
    {
        isDead = false;
        currentAimSpeed = 0;
        switch (rematch)
        {
            case 0: // initial stats
                {
                    currentExpGiven = 10;
                    maxAimSpeed = 1.5f;
                    minAimSpeed = 0.1f;
                    maxFireRate = 3;
                    minFireRate = 2;
                    bulletSpeed = 50;
                    disableShockwave = true;
                    SetHP(20);
                    break;
                }
            case 1:
                {
                    currentExpGiven = 33;
                    maxAimSpeed = 2;
                    minAimSpeed = 1.5f;
                    maxFireRate = 2;
                    minFireRate = 1.0f;
                    bulletSpeed = 75;
                    disableShockwave = false;
                    SetHP(50);
                    break;
                }
            case 2:
                {
                    currentExpGiven = 100;
                    maxAimSpeed = 5.0f;
                    minAimSpeed = 0.1f;
                    maxFireRate = 1.5f;
                    minFireRate = 0.5f;
                    bulletSpeed = 100;
                    disableShockwave = false;
                    SetHP(100);
                    break;
                }
            case 3:
                {
                    currentExpGiven = 225;
                    maxAimSpeed = 5f;
                    minAimSpeed = 2f;
                    maxFireRate = 0.75f;
                    minFireRate = 0.1f;
                    bulletSpeed = 150;
                    disableShockwave = false;
                    shockwaveDelay = 10;
                    SetHP(125);
                    break;
                }
        }
        numOfSparks = Random.Range(7, 12);
        if (!ui)
            ui = FindObjectOfType<MinibossUI>();
        ui.EnableUI();
    }

    private void SetHP(int newHP)
    {
        totalHP = newHP;
        currentHP = totalHP;
    }


    // Update is called once per frame
    private void Update()
    {
        if (!player)
            return;
        dmgTakenCounter += Time.deltaTime;
        if (dmgTakenCounter >= dmgTakenDelay)
        {
            dmgTakenCounter = 0;
            canTakeDmg = true;
            Unflash();
        }
        if (!isDead)
        {
            if (!disableShockwave)
            {
                shockwaveCounter += Time.deltaTime;
                if (shockwaveCounter >= shockwaveDelay)
                {
                    shockwaveCounter = 0;
                    ChargeShockwave();
                }
            }
            Vector3 target = player.transform.position;
            cooldown += Time.deltaTime;
            if (cooldown >= currentFireRate)
            {
                cooldown = 0;
                Fire(target);
                currentFireRate -= fireRateInterval;
                if (currentFireRate <= minFireRate)
                {
                    currentFireRate = maxFireRate;
                }
            }
            currentAimSpeed += Time.deltaTime / 5;
            if (currentAimSpeed > maxAimSpeed)
                currentAimSpeed = minAimSpeed;
            if (!isRotating)
            {
                Quaternion temp = Quaternion.LookRotation(target - cannon.transform.position, Vector3.up);
                Quaternion rotationTarget =
                    Quaternion.Euler(
                        temp.eulerAngles.x,
                        cannon.transform.rotation.eulerAngles.y,
                        cannon.transform.rotation.eulerAngles.z);
                cannon.transform.rotation = Quaternion.Slerp(cannon.transform.rotation, rotationTarget, Time.deltaTime * currentAimSpeed);
            }
            if (target.x > 0)
            {
                if (aimBody.transform.rotation.eulerAngles.y <= 90 || aimBody.transform.rotation.eulerAngles.y >= 270)
                {
                    aimBody.transform.Rotate(new Vector3(0, 0, 1), -rotationSpeed * Time.deltaTime);
                    isRotating = true;
                }
                else
                    isRotating = false;
            }
            else
            {
                if (aimBody.transform.rotation.eulerAngles.y <= 85 || aimBody.transform.rotation.eulerAngles.y > 265)
                {
                    aimBody.transform.Rotate(new Vector3(0, 0, 1), rotationSpeed * Time.deltaTime);
                    isRotating = true;
                }
                else
                    isRotating = false;
            }
        }
    }
}
