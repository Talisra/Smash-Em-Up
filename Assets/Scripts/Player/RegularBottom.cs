using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegularBottom : MonoBehaviour
{

    public GameObject jetPrefab;
    private GameObject jet;

    // Start is called before the first frame update
    void Start()
    {
        jet = Instantiate(jetPrefab, transform.position, Quaternion.identity) as GameObject;
        jet.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        jet.transform.position = transform.position;
        jet.transform.rotation = Quaternion.Slerp(
            this.GetComponentInParent<Transform>().transform.rotation, 
            this.GetComponentInParent<Transform>().transform.rotation, 0);
    }
}
