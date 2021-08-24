using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class PerkPanelUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler
{
    public bool isSkill;
    public Text powerUse;
    public Image powerup;

    [HideInInspector]
    public bool isInSlot;
    public Perk perk;
    public Image bg;
    public Image icon;
    public Text perkName;
    public Text lvl;

    public PerksDescription description_tail;
    public ScrollRect scrollRect;

    private DraggableSkill decoy;

    private PerksObserver perksObserver;
    
    void Start()
    {
        perksObserver = FindObjectOfType<PerksObserver>();
    }

    public void InitPerk(Perk newPerk)
    {
        perk = newPerk;
        isSkill = perk.isSkill ? true : false;
        icon.sprite = perk.image;
        perkName.text = perk.perkName;
        lvl.text = "Lvl. " +newPerk.currentPerkLevel.ToString();
        bg.color = isSkill ? Color.red : new Color(1,.9f,.5f);

        description_tail = Instantiate(Resources.Load("PerkDescription", typeof(PerksDescription)),
        transform.position, Quaternion.identity) as PerksDescription;
        description_tail.desc.text = perk.CreateFullDescription();
        description_tail.bg.color = new Color(0.7137255f, 1, 0.7294118f);
        description_tail.gameObject.SetActive(false);
        
        if (isSkill)
        {
            // ui
            scrollRect = transform.parent.parent.GetComponent<ScrollRect>();
            powerup.gameObject.SetActive(true);
            powerUse.gameObject.SetActive(true);
            powerUse.text = newPerk.skill.cost.ToString();

            // draggable
            decoy = Instantiate(Resources.Load("SkillDraggable", typeof(DraggableSkill)),
                transform.position, Quaternion.identity) as DraggableSkill;
            decoy.InitPerk(perk);
            decoy.SetParentUI(this);
            decoy.gameObject.SetActive(false);
        }
    }


    public void DragRelease()
    {
        scrollRect.enabled = true;
        perksObserver.DraggingSkill(null);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameProfile.Instance.ShowTutorialForPlayer(4);
        if (description_tail && perksObserver.draggableSkill == null)
            description_tail.gameObject.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
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

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isSkill || isInSlot)
            return;
        description_tail.gameObject.SetActive(false);
        scrollRect.enabled = false;
        perksObserver.DraggingSkill(decoy);
        decoy.gameObject.SetActive(true);
        decoy.DragProperties();
        eventData.pointerDrag = decoy.gameObject;
    }

    public void OnDrag(PointerEventData eventData)
    {
    }
}
