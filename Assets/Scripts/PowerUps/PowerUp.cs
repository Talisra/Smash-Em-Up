using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUp : MonoBehaviour, IPoolableObject
{
    public float despawnDelay;
    public GameObject pickParticle;
    public string pickSoundString;
    protected Player player;
    protected AudioManager audioManager;
    private float despawnCounter = 0;
    // Start is called before the first frame update
    void Awake()
    {
        player = FindObjectOfType<Player>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.parent)
            if (other.gameObject.transform.parent.tag == "Player")
            {
                Collect();
            }
    }

    // Update is called once per frame
    void Update()
    {
        despawnCounter += Time.deltaTime;
        if (despawnCounter >= despawnDelay)
        {
            Despawn();
        }
    }

    public virtual void Collect()
    {

    }

    public virtual void Despawn()
    {
        gameObject.SetActive(false);
        despawnCounter = 0;
        GameObject effect = Instantiate(pickParticle, transform.position, Quaternion.identity) as GameObject;
        Destroy(effect, 0.5f);
        Invoke("BackToPool", 0.3f);
    }

    public virtual void BackToPool()
    {
        //Implement at Inherited powerup
    }
}
