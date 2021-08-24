using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class x_observer : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image bg;


    void OnEnable()
    {
        bg.color = Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        bg.color = Color.grey;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        bg.color = Color.white;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        transform.parent.parent.gameObject.SetActive(false);
    }

}
