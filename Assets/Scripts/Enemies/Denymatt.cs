using System.Collections;
using UnityEngine;

public class Denymatt : Enemy
{
    public float maxTimeThreshold;
    public float minTimeThreshold;
    private float countdownThreshold;
    private float countdownAudioThreshold;
    private bool hasPlayedAudio = false;
    private float currentCountdown = 0;

    public GameObject spark;
    private float sparkSize = 1;

    private float thrustThreshold;
    private bool hasThrusted = false;

    public string countdownSound;
    private float countdownAudioLength;

    protected override void Update()
    {
        base.Update();
        thrustCounter = 0; // Ignore thrust from base class Enemy
        currentCountdown += Time.deltaTime;
        if ((currentCountdown >= thrustThreshold) && !hasThrusted)
        {
            hasThrusted = true;
            if (player)
            {
                Thrust(player.transform.position);
            }
        }
        if ((currentCountdown >= countdownAudioThreshold) && !hasPlayedAudio)
        {
            hasPlayedAudio = true;
            spark.SetActive(true);
            AudioManager.Instance.Play(countdownSound);
            AudioManager.Instance.Play("Fuse");
        }
        if (currentCountdown >= countdownThreshold)
        {
            Damage(GetCurrentHp());
        }
        if (spark.activeSelf)
        {
            sparkSize += Time.deltaTime *1.5f;
            spark.transform.localScale = new Vector3(sparkSize, sparkSize, sparkSize);
        }
    }

    protected override void BeforeExplode()
    {
        spark.SetActive(false);
        sparkSize = 1;
        AudioManager.Instance.Stop(countdownSound);
        if (Vector3.Distance(player.transform.position, transform.position) < 8f)
        {
            player.TakeTrueDamage(2, true, false, Vector3.zero);
        }
    }

    private void SetBomb()
    {
        countdownThreshold = Random.Range(minTimeThreshold, maxTimeThreshold);
        thrustThreshold = countdownThreshold - 0.4f;
        countdownAudioLength = AudioManager.Instance.GetPitch(countdownSound) * AudioManager.Instance.GetLength(countdownSound);
        countdownAudioThreshold = countdownThreshold - countdownAudioLength;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SetBomb();
        isThrusting = false;
        hasPlayedAudio = false;
        hasThrusted = false;
        currentCountdown = 0;
    }

    public override void BackToPool()
    {
        base.BackToPool();
        DenymattPool.Instance.ReturnToPool(this);
    }
}
