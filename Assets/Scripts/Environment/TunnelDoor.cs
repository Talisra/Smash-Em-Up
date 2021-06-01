using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelDoor : MonoBehaviour
{
    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OpenDoors()
    {
        animator.Play("Open");
    }

    public void CloseDoors()
    {
        animator.Play("Close");
    }
}
