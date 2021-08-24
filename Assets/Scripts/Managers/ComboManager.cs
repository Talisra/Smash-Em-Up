using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    public GameObject comboPrefab;
    private int comboCounter = 0;

    private float baseDelay = 5;
    private float incrementalDelay = 0.95f;

    private float delayCounter;
    private float maxDelay;

    private int maxComboFromProfile = 0;

    private void Start()
    {
        maxDelay = baseDelay;
        delayCounter = maxDelay;
    }

    public void SetMaxCombo(int comb)
    {
        maxComboFromProfile = comb;
    }

    public float GetCurrentDelay()
    {
        return delayCounter;
    }

    public float GetMaxDelay()
    {
        return maxDelay;
    }

    public int GetComboCounter()
    {
        return comboCounter;
    }

    public void AddCombo()
    {
        comboCounter++;
        if (comboCounter > maxComboFromProfile)
        {
            ResetCombo();
        }
        if (comboCounter == 5)
        {
            //audioManager.Play("Combo5");
            GameManager.Instance.AddPowerup(1);
        }
        maxDelay = baseDelay*incrementalDelay;
        delayCounter = maxDelay;
        if (comboCounter == 10)
        {
            //audioManager.Play("Combo5");
            GameManager.Instance.AddPowerup(2);
            ResetCombo();
        }
    }

    public void ResetCombo()
    {
        maxDelay = baseDelay;
        comboCounter = 0;
        delayCounter = maxDelay;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (comboCounter > 0)
        {
            delayCounter-= Time.deltaTime;
            if (delayCounter <= 0)
                ResetCombo();
        }

    }
}
