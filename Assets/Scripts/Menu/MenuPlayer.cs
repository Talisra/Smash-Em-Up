using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPlayer : MonoBehaviour
{
    public GameObject smashAnimPrefab;

    private Rigidbody rb;
    private Animator animator;
    private float targetY;
    private Vector3 accelerationVector = Vector3.zero;
    private bool isBusy = false;


    // Start is called before the first frame update
    void Start()
    {
        targetY = transform.position.y;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    public void AlignY(float newY)
    {
        targetY = newY - 0.25f;
    }

    public void SmashMenuObject()
    {
        isBusy = true;
        animator.Play("AtkLeft");
        FindObjectOfType<AudioManager>().Play("Smash1");
        GameObject particle = Instantiate(
            smashAnimPrefab, 
            new Vector3(transform.position.x - 1.25f, transform.position.y, transform.position.z),
            Quaternion.identity) as GameObject;
        ParticleSystem parts = particle.GetComponent<ParticleSystem>();
        Destroy(particle, parts.main.duration);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isBusy)
        {
            transform.position = Vector3.SmoothDamp(transform.position,
                new Vector3(transform.position.x, targetY, transform.position.z),
                ref accelerationVector, 0.1f);
        }
    }

}
