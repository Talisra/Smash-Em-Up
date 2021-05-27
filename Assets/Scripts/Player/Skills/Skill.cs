using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    public bool isCharge; // decides if the skill is just one click or click and hold.
    public bool isMultiHit;
    public int cost;
    public string animationName;
    public float invTime;
    public bool takeControl;
    public GameObject hitPrefab;

    protected Player player;

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
    }

    public virtual void OnSmash(Enemy enemy)
    {

    }
    public virtual void OnWallCollision(Collision collision)
    {

    }

    public virtual void OnMapBonusCollision(MapBonus mapBonus)
    {

    }

    public virtual void OnInputRelease()
    {

    }

    public virtual void OnStartAction()
    {

    }
    public virtual void OnEndAction()
    {

    }
}
