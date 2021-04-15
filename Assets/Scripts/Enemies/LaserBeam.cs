using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    public GameObject laserImpactPrefab;
    private GameObject laserImpact;
    private LineRenderer lr;
    private CapsuleCollider capCollider;
    private GameManager gameManager;
    //private float fireTime = 0.6f;
    //private int laserLength = 5000;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        capCollider = GetComponent<CapsuleCollider>();
        gameObject.SetActive(false);
        lr = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        laserImpact = Instantiate(
            laserImpactPrefab, transform.position, Quaternion.identity) as GameObject;
    }

    private void OnDisable()
    {
        if (laserImpact)
            laserImpact.SetActive(false);
    }

    private void OnEnable()
    {
        if (laserImpact)
        {
            laserImpact.SetActive(true);
        }
    }


    // Update is called once per frame
    void LateUpdate()
    {
        lr.SetPosition(0, transform.position);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, 
            transform.TransformDirection(new Vector3(0,0,1)),
            out hit))
        {
            if (hit.collider)
            {
                if (hit.collider.tag == "PlayerBody")
                {
                    Player player = hit.collider.gameObject.GetComponentInParent<Player>();
                    player.TakeDamage();
                }
                laserImpact.transform.position = hit.point;
                lr.SetPosition(1, hit.point);
            }
        }
        //capCollider.center = (lr.GetPosition(1) + lr.GetPosition(1)) / 2;
        //capCollider.height = Vector3.Distance(lr.GetPosition(0), lr.GetPosition(1));
        /*
        else
            lr.SetPosition(1, new Vector3(0,0, laserLength));*/
    }

    private void OnDestroy()
    {
        Destroy(laserImpact);
    }
}
