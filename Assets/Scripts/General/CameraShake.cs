using System.Collections;
using UnityEngine;
public class CameraShake : MonoBehaviour
{
    public static VHSPostProcessEffect glitchEffect;
    public static CameraShake instance;

    private Vector3 _originalPos;
    private float _timeAtCurrentFrame;
    private float _timeAtLastFrame;
    private float _fakeDelta;

    void Awake()
    {
        glitchEffect = GetComponent<VHSPostProcessEffect>();
        instance = this;
    }

    void Update()
    {
        // Calculate a fake delta time, so we can Shake while game is paused.
        _timeAtCurrentFrame = Time.realtimeSinceStartup;
        _fakeDelta = _timeAtCurrentFrame - _timeAtLastFrame;
        _timeAtLastFrame = _timeAtCurrentFrame;
    }


    public static void Shake(float duration, float amount)
    {
        instance._originalPos = instance.gameObject.transform.localPosition;
        instance.StopAllCoroutines();
        instance.StartCoroutine(instance.cShake(duration, amount));
        instance.Glitch(duration, amount);
    }

    public static void EndGameEffectGlitch()
    {
        glitchEffect.SetScan(true);
    }

    private void Glitch(float duration, float amount)
    {
        if (duration >= 0.5f)
        {
            AudioManager.Instance.Play("WhiteNoise");
            glitchEffect.enabled = true;
            if (!glitchEffect._xScan)
                Invoke("StopGlitch", duration*0.75f);
        }
    }

    private void StopGlitch()
    {
        AudioManager.Instance.Stop("WhiteNoise");
        glitchEffect.enabled = false;
        glitchEffect.SetScan(false);
    }

    public IEnumerator cShake(float duration, float amount)
    {
        float endTime = Time.time + duration;

        while (duration > 0)
        {
            transform.localPosition = _originalPos + Random.insideUnitSphere * amount;

            duration -= _fakeDelta;

            yield return null;
        }

        transform.localPosition = _originalPos;
    }

}