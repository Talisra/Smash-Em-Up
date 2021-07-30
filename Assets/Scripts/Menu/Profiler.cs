using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Profiler : MonoBehaviour
{
    public Image expBar;
    public Image expBarFill;
    public Text playerName;
    public Text level;
    public List<Renderer> ledRend;
    public Color profilerColor = Color.blue;
    private Profile playerProfile;

    protected virtual void Start()
    {
        playerProfile = null;
        expBar.color = profilerColor;
        expBarFill.color = profilerColor;
        playerName.color = profilerColor;
        level.color = profilerColor;
        foreach(Renderer rend in ledRend)
        {
            rend.material.SetColor("_Color", profilerColor);
            rend.material.SetColor("_EmissionColor", profilerColor);
        }
        UpdateUI();
    }

    public void SetProfile(Profile profile)
    {
        playerProfile = profile;
    }

    public void UpdateUI()
    {
        if (playerProfile == null)
        {
            playerName.text = " ";
            level.text = " ";
            expBarFill.fillAmount = 0;
        }
        else
        {
            playerName.text = playerProfile.profileName;
            level.text = "Level " + playerProfile.level.ToString();
            expBarFill.fillAmount = ((float)playerProfile.currentExp / (float)playerProfile.expToNext);
        }
    }
}
