using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairSkill : Skill
{
    private int healAmount = 2;
    public GameObject repairEffect;

    private bool isActive = false;
    private float effectCounter = 0;
    private float effectDelay = .25f;
    private bool soundPlayed = false;
    public override void OnStartAction()
    {
        soundPlayed = false;
        AudioManager.Instance.Play("Repair1");
        player.animator.Play(animationName);
        player.TakeControl();
        isActive = true;
    }

    public override void OnEndAction()
    {
        isActive = false;
        player.Heal(healAmount);
        player.GiveControl();
        AudioManager.Instance.Play("Heal");
    }

    private void Update()
    {
        if (isActive)
        {
            effectCounter += Time.deltaTime;
            if (effectCounter > effectDelay && effectCounter < 1.5f)
            {
                if (!soundPlayed)
                {
                    AudioManager.Instance.Play("Repair2");
                    soundPlayed = true;
                }
                Vector3 spawnVector = new Vector3(player.transform.position.x, player.transform.position.y + Random.Range(-.5f, .5f), player.transform.position.z -.5f);
                GameObject effect = Instantiate(repairEffect, spawnVector, Quaternion.identity) as GameObject;
                Destroy(effect, 1);
                effectCounter = 0;
            }
        }
    }
}
