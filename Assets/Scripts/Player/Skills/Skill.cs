using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    public int cost;
    public string animationName;
    public float invTime;
    public bool takeControl;
    public GameObject hitPrefab;

    protected Player player;
    protected AudioManager audioManager;

    protected void ShowHitParticle(Transform pos)
    {
        GameObject particle = Instantiate(
            hitPrefab, pos.position, Quaternion.identity) as GameObject;
        ParticleSystem parts = particle.GetComponent<ParticleSystem>();
        Destroy(particle, parts.main.duration);
    }

    private void Start()
    {
        player = FindObjectOfType<Player>();
        audioManager = FindObjectOfType <AudioManager>();
    }

    public virtual void OnSmash(Enemy enemy)
    {

    }
    public virtual void OnWallCollision(Collision collision)
    {

    }

    public virtual void OnStartAction()
    {

    }
    public virtual void OnEndAction()
    {

    }
}
