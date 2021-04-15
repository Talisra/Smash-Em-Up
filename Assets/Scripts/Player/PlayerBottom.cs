using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBottom : MonoBehaviour
{
    public Texture normalTex;
    public Texture dmgTex;
    private Renderer rend;

    private bool isDamaged = false;
    private float dmgTime = 0.4f;
    private float dmgTimeCounter = 0;

    // Start is called before the first frame update
    void Awake()
    {
        rend = GetComponent<Renderer>();
    }
    public void ShowDamage()
    {
        isDamaged = true;
        rend.material.SetTexture("_MainTex", dmgTex);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDamaged)
        {
            dmgTimeCounter += Time.deltaTime;
            if (dmgTimeCounter > dmgTime)
            {
                dmgTimeCounter = 0;
                isDamaged = false;
                rend.material.SetTexture("_MainTex", normalTex);
            }
        }
    }
}
