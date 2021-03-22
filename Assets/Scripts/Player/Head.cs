using UnityEngine;

public class Head : MonoBehaviour
{
    public GameObject leftH;
    public GameObject rightH;
    public BoxCollider[] colliders;

    private float headBigHitBox = 2.0f;

    private float headNormalHitboxX = 1.33f;
    private float headNormalHitboxY = 0.67f;

    private void Start()
    {
        colliders = GetComponents<BoxCollider>();
    }

    public void ManageAttack(float delay)
    {
        ManageAtkTrail();
        ManageHitBox();
        Invoke("StopAttack", delay);
    }

    public void StopAttack()
    {
        RevertHitBox();
        StopAtkTrail();
    }

    public void ManageHitBox()
    {
        foreach(BoxCollider collider in colliders)
        {
            collider.size = new Vector3(headBigHitBox, headBigHitBox, 1);
        }
    }

    public void ManageAtkTrail()
    {
        leftH.GetComponent<TrailRenderer>().enabled = true;
        rightH.GetComponent<TrailRenderer>().enabled = true;
    }

    private void RevertHitBox()
    {
        foreach (BoxCollider collider in colliders)
        {
            collider.size = new Vector3(headNormalHitboxX, headNormalHitboxY, 1);
        }
    }

    private void StopAtkTrail()
    {
        leftH.GetComponent<TrailRenderer>().enabled = false;
        rightH.GetComponent<TrailRenderer>().enabled = false;
    }


}
