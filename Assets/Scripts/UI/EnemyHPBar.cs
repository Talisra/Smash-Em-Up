using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBar : MonoBehaviour
{
    public Enemy enemy;
    public Slider slider;

    private float currentHp;
    private float maxHp;

    public void AttatchToEnemy(Enemy enemy)
    {
        this.enemy = enemy;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy)
        {
            currentHp = enemy.GetCurrentHp();
            maxHp = enemy.GetMaxHp();
            slider.value = currentHp / maxHp;
            transform.position = new Vector3(enemy.transform.position.x, enemy.transform.position.y, 0);
        }

    }
}
