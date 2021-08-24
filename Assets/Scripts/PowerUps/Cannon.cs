using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MapBonus, IPoolableObject
{
    public GameObject cannonAnchor;

    public GameObject particleAnchor;
    public GameObject fireParticle;
    [Range(1, 4)]
    public int direction; // to see on editor
    [Range(1, 3)]
    public int size; // to see on editor
    private int eulerDirection; // actual convert for easy use

    private Collider[] colliders;
    private Animator animator;
    private AudioManager audioManager;
    private float launchDelay = 1f;
    private Enemy storedEnemy;
    private bool isBusy = false;
    private bool isAdjusting = false;
    private float deltaRotation;
    private float launchPower;
    private Vector3 launchVector;

    // Start is called before the first frame update
    void Awake()
    {
        eulerDirection = direction * 90 - 90;
        launchVector = SetLaunchVector();
        animator = cannonAnchor.GetComponent<Animator>();
        audioManager = FindObjectOfType<AudioManager>();
        colliders = cannonAnchor.GetComponents<Collider>();
        transform.rotation = Quaternion.Euler(0,0, eulerDirection);
    }

    public void SetCannonProperties(int direction, int size)
    {
        this.direction = direction;
        this.size = size;
    }
    public void Spawn()
    {
        ResetCannon();
        animator.Play("Spawn");
    }

    private void ResetCannon() // reset the cannon with new size/direction values and prepare it to work
    {
        isBusy = false;
        foreach (Collider c in colliders)
            c.enabled = true;
        eulerDirection = direction * 90 - 90;
        launchVector = SetLaunchVector();
        transform.rotation = Quaternion.Euler(0, 0, eulerDirection);
        float scale = 0.1f;
        switch(size)
        {
            case 1:
                scale = 0.1f;
                launchPower = 60;
                break;
            case 2:
                scale = 0.13f;
                launchPower = 100;
                break;
            case 3:
                scale = 0.16f;
                launchPower = 150;
                break;
        }
        Vector3 scaleVector = new Vector3(scale, scale, scale);
        transform.localScale = scaleVector;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            if (!isBusy)
            {
                Store(other.gameObject.GetComponent<Enemy>());
                isBusy = true;
            }
        }
    }

    private void Store(Enemy enemy)
    {
        // look at player to see how to do it properly
        Vector3 difference = transform.position - enemy.transform.position;
        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
        animator.Play("Cannon_Eat");
        storedEnemy = enemy;
        Boule boule;
        if (enemy.TryGetComponent<Boule>(out boule))
        {

            boule.HitByPlayer();
        }
        storedEnemy.tempDisable = true;
        storedEnemy.gameObject.SetActive(false);
        storedEnemy.StopMovement();
        storedEnemy.transform.position = this.transform.position;
        isAdjusting = true;
        SetDeltaRotation();
        Invoke("Launch", launchDelay);
    }

    private void Launch()
    {
        audioManager.Play("Cannon_8bit");
        audioManager.Play("Cannon_Norm");
        GameObject explosion = Instantiate(fireParticle, particleAnchor.transform.position, transform.rotation) as GameObject;
        ParticleSystem particle = explosion.GetComponent<ParticleSystem>();
        Destroy(explosion, 0.4f);
        foreach (Collider c in colliders)
            c.enabled = false;
        animator.Play("Cannon_Fire");
        storedEnemy.gameObject.SetActive(true);
        storedEnemy.GetComponent<Rigidbody>().AddForce(launchVector.normalized * launchPower, ForceMode.VelocityChange);
        storedEnemy.GiveSuperSpeed(2);
        storedEnemy = null;
        Invoke("Despawn", 0.5f);
    }

    public override void Despawn()
    {
        CancelInvoke();
        if (storedEnemy != null)
        {
            storedEnemy.gameObject.SetActive(true);
        }
        animator.Play("Despawn");
        BackToPool();
    }

    private void Update()
    {
        if (isAdjusting)
        { 
            if (transform.rotation.eulerAngles.z - eulerDirection < 1)
            {
                isAdjusting = false;
                transform.rotation = Quaternion.Euler(0, 0, eulerDirection);
            }
  
            transform.Rotate(new Vector3(0, 0, 1), deltaRotation * Time.deltaTime);
        }
    }
    
    private Vector3 SetLaunchVector()
    {
        switch (direction)
        {
            case 1:
                {
                    return new Vector3(-1,0,0);
                }
            case 2:
                {
                    return new Vector3(0,-1,0);
                }
            case 3:
                {
                    return new Vector3(1,0,0);
                }
            case 4:
                {
                    return new Vector3(0,1,0);
                }
        }
        return Vector3.zero;
    }

    private void SetDeltaRotation()
    {
        float interval = Mathf.Min(
            eulerDirection - transform.rotation.eulerAngles.z,
            transform.rotation.eulerAngles.z - eulerDirection);
        deltaRotation = interval / (launchDelay - 0.2f);

    }

    public void BackToPool()
    {
        CannonPool.Instance.ReturnToPool(this);
    }
}
