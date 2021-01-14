using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    public GameObject comboPrefab;
    public GameManager gameManager;
    private int comboCounter = 0;

    private int baseDelay = 1000;
    private float incrementalDelay = 0.95f;

    private int currentDelay;
    private int maxDelay;

    private void Start()
    {
        maxDelay = baseDelay;
        currentDelay = maxDelay;
    }

    public int GetCurrentDelay()
    {
        return currentDelay;
    }

    public int GetMaxDelay()
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
        maxDelay = (int)(baseDelay*incrementalDelay);
        currentDelay = maxDelay;

    }

    private void ResetCombo()
    {
        gameManager.AddScore(comboCounter * (1 + comboCounter/10));
        maxDelay = baseDelay;
        comboCounter = 0;
        currentDelay = maxDelay;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (comboCounter > 0)
        {
            currentDelay--;
            if (currentDelay <= 0)
                ResetCombo();
        }

    }
}
