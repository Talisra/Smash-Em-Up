using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilingDoor : MonoBehaviour
{
    private Animator animator;
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

}
