using UnityEngine;
using UnityEngine.UI;

public class ComboBar : MonoBehaviour
{
    private Image comboBarImg;
    public float currentGauge;
    private float maxGauge;

    ComboManager comboManager;

    // Start is called before the first frame update
    void Start()
    {
        comboBarImg = GetComponent<Image>();
        comboManager = FindObjectOfType<ComboManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (comboManager.GetComboCounter() > 0)
        {
            currentGauge = comboManager.GetCurrentDelay();
            maxGauge = comboManager.GetMaxDelay();
            comboBarImg.fillAmount = currentGauge / maxGauge;
        }
        else
            comboBarImg.fillAmount = 0;
    }
}
