using UnityEngine.UI;
using UnityEngine;

public class PowerupCounterText : MonoBehaviour
{
    private Text text;
    Player player;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = player.GetCurrentPowerUps().ToString();
    }
}
