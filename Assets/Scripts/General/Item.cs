using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public PerksObserver perksObserver;
    public GameObject hoverEffect;
    private void OnMouseEnter()
    {
        hoverEffect.gameObject.SetActive(true);
    }

    private void OnMouseExit()
    {
        hoverEffect.gameObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        perksObserver.gameObject.SetActive(true);
    }
}
