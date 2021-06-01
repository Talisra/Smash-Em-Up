using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    public GameObject laserImpactPrefab;
    private GameObject laserImpact;
    private LineRenderer lr;
    private CapsuleCollider capCollider;
    //private float fireTime = 0.6f;
    //private int laserLength = 5000;

    private void Awake()
    {
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

    private bool checkCanFire()
    {
        List<float> area = GameManager.Instance.GetGameArea();
        if (transform.position.x < area[0] || transform.position.x > area[2])
        {
            return false;
        }
        if (transform.position.y < area[1] || transform.position.y > area[3])
        {
            return false;
        }
        return true;
    }

    void LateUpdate()
    {
        if (checkCanFire())
        {
            lr.SetPosition(0, transform.position);
            RaycastHit hit;
            if (Physics.Raycast(transform.position,
                transform.TransformDirection(new Vector3(0, 0, 1)),
                out hit))
            {
                if (hit.collider)
                {
                    if (hit.collider.tag == "PlayerBody")
                    {
                        Player player = hit.collider.gameObject.GetComponentInParent<Player>();
                        player.TakeDamage(1, true, true, hit.point);
                    }
                    laserImpact.transform.position = hit.point;
                    lr.SetPosition(1, hit.point);
                }
            }
        }
        else
        {
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, transform.position);
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
