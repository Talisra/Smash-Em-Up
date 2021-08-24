using UnityEngine.UI;
using UnityEngine;

public class PerkPanel : MonoBehaviour
{
    public Perk perk;
    public Image bg;
    public Image icon;
    public Text perkName;
    public Text lvl;

    public PerksDescription description_tail;

    public void InitPerk(Perk newPerk)
    {
        perk = newPerk;
        icon.sprite = perk.image;
        perkName.text = perk.perkName;
        lvl.text = "Lvl. " +newPerk.currentPerkLevel.ToString();
        bg.color = perk.isSkill ? Color.blue : Color.red;
        description_tail = Instantiate(Resources.Load("PerkDescription", typeof(PerksDescription)),
            transform.position, Quaternion.identity) as PerksDescription;
        description_tail.desc.text = perk.CreateFullDescription();
        description_tail.bg.color = perk.isSkill ? Color.blue : Color.cyan;
        description_tail.gameObject.SetActive(false);
    }

    private void OnMouseEnter()
    {
        if (description_tail)
            description_tail.gameObject.SetActive(true);
    }

    private void OnMouseExit()
    {
        if (description_tail)
            description_tail.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (description_tail)
        {
            Vector3 targetPos = Input.mousePosition;
            description_tail.box.transform.position = new Vector3(
                targetPos.x + description_tail.bg.rectTransform.rect.width/2,
                targetPos.y - description_tail.bg.rectTransform.rect.height/2,
                0
                );
        }
    }

}
