using UnityEngine;

public class Head : MonoBehaviour
{
    public GameObject leftH;
    public GameObject rightH;


    public void ManageAtkTrail(float delay)
    {
        leftH.GetComponent<TrailRenderer>().enabled = true;
        rightH.GetComponent<TrailRenderer>().enabled = true;
        Invoke("StopAtkTrail", delay);
    }

    private void StopAtkTrail()
    {
        leftH.GetComponent<TrailRenderer>().enabled = false;
        rightH.GetComponent<TrailRenderer>().enabled = false;
    }

}
