using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrinatiShield : MonoBehaviour
{
    private int maxHP = 3;
    public GameObject effect;
    public GameObject scrapPrefab;
    private TrinatiShieldScrap shieldScrap;


    private void Start()
    {
        shieldScrap = Instantiate(scrapPrefab, transform.position, Quaternion.identity).GetComponent<TrinatiShieldScrap>();
        shieldScrap.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        maxHP = 3;
    }

    public void Damage()
    {
        GameObject temp = Instantiate(effect, transform.position, Quaternion.identity) as GameObject;
        Destroy(temp, 1);
        AudioManager.Instance.Play("TrinatiShieldDamage");
        maxHP--;
        if (maxHP <=0)
        {
            Disarm();
        }
    }

    private void Disarm()
    {
        AudioManager.Instance.Play("TrinatiShieldDead");
        shieldScrap.gameObject.SetActive(true);
        shieldScrap.transform.position = transform.position;
        shieldScrap.StartFadeOut();
        gameObject.SetActive(false);
    }
}
