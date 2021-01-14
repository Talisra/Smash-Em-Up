using UnityEngine;
using UnityEngine.UI;

public class ComboCounterText : MonoBehaviour
{
    private Text text;
    ComboManager comboManager;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        text.enabled = false;
        comboManager = FindObjectOfType<ComboManager>();
    }

    // Update is called once per frame
    void Update()
    {
        int comboCounter = comboManager.GetComboCounter();
        if (comboCounter > 0)
        {
            text.enabled = true;
            text.text = comboCounter.ToString();
        }
        else
            text.enabled = false;
    }
}
