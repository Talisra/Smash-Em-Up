using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachPoint : MonoBehaviour
{
    [HideInInspector]
    public List<AIobstacle> obstacles = new List<AIobstacle>();

    public Tunnel tunnelBlocked;

    public bool isHorizontal;
    private int direction;

    public bool isBusy = false;
    private int counter;
    private int total;

    private Vector3 nextAttachPosition;

    void Start()
    {
        direction = (int)Mathf.Sign(transform.position.x);
        nextAttachPosition = transform.position;
    }

    public void AttachObstacles(List<AIobstacle> aios)
    {
        if (tunnelBlocked) // if there is attached tunnel, its blocked and cannot summon
            tunnelBlocked.isBlocked = true;
        isBusy = true;
        total = aios.Count;
        if (obstacles.Count > 0 || aios.Count == 0)
        {
            return; // can only add a list once, and cannot add an empty list
        }
        obstacles.AddRange(aios);
        aios[0].SetAttachPoint(this, transform.position);
        //Debug.Log("incoming target: " + nextAttachPosition);
        if (aios.Count > 1)
        {
            for (int i = 1; i < aios.Count; i++)
            {
                nextAttachPosition += new Vector3(
                    isHorizontal ?
                        (aios[0].transform.localScale.x * -direction) : (0),
                    isHorizontal ? 
                        (0) : (-aios[0].transform.localScale.y), // NO ATTACH POINTS AT BOTTOM!
                    0);
                //Debug.Log("incoming target: " +nextAttachPosition);
                aios[i].SetAttachPoint(this, nextAttachPosition);
            }
        }
    }

    public void AIOmessage()
    {
        counter++;
        if (counter == total)
        {
            isBusy = false;
            AIObstacleManager.Instance.AIOmessage();
        }
    }

    public bool CheckIfBusy()
    {
        return isBusy;
    }

    public void DetachAll()
    {
        for (int i=obstacles.Count-1 ; i >= 0 ; i--)
        {
            obstacles[i].SpawnOut();
        }
        obstacles.Clear();
        nextAttachPosition = transform.position;
        counter = 0;
        if (tunnelBlocked)
            tunnelBlocked.isBlocked = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
