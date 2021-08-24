using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class EXPwindow : MonoBehaviour
{
    public Text username;
    public Text lvl;
    public Text extraEXPpool;
    public Text countEXP;
    public Image EXPgauge;

    public GameObject lvlupParticle;
    public GameObject starParticle;
    public GameObject starPos;

    //private float soundCounter = 0;
    //private float soundDelay = 0.22f;
    private float changeSpeed = 5;
    private Profile profile;
    private float expPool;
    private float currentEXP;

    bool hasInvoked = false;

    public void Init(float extraEXP, Profile profile)
    {
        hasInvoked = false;
        this.profile = profile;
        currentEXP = profile.currentExp;
        expPool = extraEXP;
        extraEXPpool.text = Mathf.CeilToInt(expPool).ToString();
        // GUI
        lvl.text = "Level " + profile.level.ToString();
        username.text = profile.profileName;
        EXPgauge.fillAmount = currentEXP / profile.expToNext;
    }

    void LevelUP()
    {
        MenuManager.Instance.RefreshProfile(profile);
        profile.LevelUP();
        GameObject effect = Instantiate(lvlupParticle, transform.position, Quaternion.identity) as GameObject;
        GameObject effect2 = Instantiate(starParticle, starPos.transform.position, Quaternion.identity) as GameObject;
        AudioManager.Instance.Play("LvlupBIT");
        AudioManager.Instance.Play("LvlupSFX");
        Destroy(effect, 4);
        Destroy(effect2, 1);
        Init(expPool, profile);
    }

    void Update()
    {
        // Level UP
        if (currentEXP >= profile.expToNext)
        {
            LevelUP();
        }
        if (expPool > 0)
        {
            changeSpeed += Time.deltaTime * 5;
            AudioManager.Instance.Play("ExpCount");
            // Calculations
            float amount = (float)(Time.deltaTime * changeSpeed);
            currentEXP += amount;
            expPool -= amount;
            // GUI
            extraEXPpool.text = "+" +(Mathf.CeilToInt(expPool)).ToString();
            if (Mathf.CeilToInt(expPool) <= 0)
                extraEXPpool.text = "";
            countEXP.text = ((int)currentEXP).ToString() + "/" + profile.expToNext.ToString();
            EXPgauge.fillAmount = currentEXP / profile.expToNext;
        }
        else
        {
            if (!hasInvoked)
            {
                Invoke("Dispose", 1.5f);
                hasInvoked = true;
            }
        }
    }

    private void Dispose()
    {
        profile.currentExp = Mathf.CeilToInt(currentEXP);
        MenuManager.Instance.RefreshProfile(profile);
        MenuManager.Instance.CheckPerks();
        GameProfile.Instance.ClearExpPool();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        this.gameObject.SetActive(false);
    }
}
