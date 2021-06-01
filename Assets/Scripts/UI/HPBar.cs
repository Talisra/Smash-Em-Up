using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public Player player;

    private Image HPimg;
    private float currentHp;
    private float maxHp;


    // Start is called before the first frame update
    void Start()
    {
        HPimg = GetComponent<Image>();
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        currentHp = player.GetCurrentHp();
        maxHp = player.GetMaxHp();
        HPimg.fillAmount = currentHp / maxHp;
    }
}
