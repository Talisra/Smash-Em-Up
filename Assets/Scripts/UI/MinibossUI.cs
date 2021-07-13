using UnityEngine.UI;
using UnityEngine;

public class MinibossUI : MonoBehaviour
{
    private Miniboss miniboss;
    public SevenSegment hpCounter;
    public Text[] hpCounterBackground;
    public Image hpBar;
    public Image hpBack;

    private float currentHp;
    private float maxHp;

    private void Start()
    {
        miniboss = FindObjectOfType<Miniboss>();
        DisableUI();
    }
    public void EnableUI()
    {
        if (!miniboss)
            miniboss = FindObjectOfType<Miniboss>();
        hpBar.enabled = true;
        hpBack.enabled = true;
        hpCounter.enabled = true;
        foreach (Text text in hpCounter.segments)
        {
            text.enabled = true;
        }
        foreach (Text text in hpCounterBackground)
        {
            text.enabled = true;
        }
    }

    public void DisableUI()
    {
        hpBar.enabled = false;
        hpBack.enabled = false;
        foreach (Text text in hpCounter.segments)
        {
            text.enabled = false;
        }
        foreach (Text text in hpCounterBackground)
        {
            text.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (miniboss)
        {
            hpCounter.SetNumber(miniboss.currentHP);
            currentHp = miniboss.currentHP;
            maxHp = miniboss.totalHP;
            hpBar.fillAmount = currentHp / maxHp;
        } 
    }
}
