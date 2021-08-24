using UnityEngine;
using UnityEngine.UI;

public class TutorialCanvas : MonoBehaviour
{
    public Text dummyText;
    public Image tutoriaImage;
    public Sprite[] tutorials;

    public void ShowTutorialScreen(int type)
    {
        switch (type)
        {
            case 0: // Start Game for the first time
                {
                    tutoriaImage.sprite = tutorials[0];
                    break;
                }
            case 1: // Skill Window can be clicked
                {
                    tutoriaImage.sprite = tutorials[2];
                    break;
                }
            case 2: // What are power ups?
                {
                    tutoriaImage.sprite = tutorials[3];
                    break;
                }
            case 3: // explanation on skill observer and skill equip
                {
                    tutoriaImage.sprite = tutorials[4];
                    break;
                }
            case 4: // explanation on skillpanelUI
                {
                    tutoriaImage.sprite = tutorials[5];
                    break;
                }
            case 5: // Damage
                {
                    tutoriaImage.sprite = tutorials[1];
                    break;
                }
        }
    }
}
