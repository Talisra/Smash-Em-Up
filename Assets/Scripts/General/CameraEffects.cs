using System.Collections;
using UnityEngine;
public class CameraEffects : MonoBehaviour
{
    public static VHSPostProcessEffect glitchEffect;
    public static Kino.AnalogGlitch glitch_analog;
    public static Kino.DigitalGlitch glitch_digital;
    public static ShutdownEffect shutdownEffect;
    public static CameraEffects instance;

    private Vector3 _originalPos;
    private float _timeAtCurrentFrame;
    private float _timeAtLastFrame;
    private float _fakeDelta;

    private static bool endGameEffect = false;

    void Awake()
    {
        glitchEffect = GetComponent<VHSPostProcessEffect>();
        shutdownEffect = GetComponent<ShutdownEffect>();
        glitch_analog = GetComponent<Kino.AnalogGlitch>();
        glitch_digital = GetComponent<Kino.DigitalGlitch>();
        glitch_digital.intensity = 0;
        glitch_analog.scanLineJitter = 0;
        instance = this;
    }

    private void Start()
    {
        endGameEffect = false;
        glitch_digital.intensity = 0;
        glitch_analog.scanLineJitter = 0;
    }

    void Update()
    {
        // Calculate a fake delta time, so we can Shake while game is paused.
        _timeAtCurrentFrame = Time.realtimeSinceStartup;
        _fakeDelta = _timeAtCurrentFrame - _timeAtLastFrame;
        _timeAtLastFrame = _timeAtCurrentFrame;
        if (endGameEffect)
        {
            glitch_analog.scanLineJitter += Time.deltaTime/5;
            if (glitch_digital.intensity > 0.5f)
            {
                glitch_digital.intensity += Time.deltaTime;
            }
            else
                glitch_digital.intensity += Time.deltaTime/5;
            if (glitch_digital.intensity > 0.99f)
                glitch_digital.intensity = 0.99f;
        }
    }


    public static void Shake(float duration, float amount)
    {
        instance._originalPos = instance.gameObject.transform.localPosition;
        instance.StopAllCoroutines();
        instance.StartCoroutine(instance.cShake(duration, amount));
        instance.Glitch(duration, amount);
    }

    public static void ShutDown()
    {
        shutdownEffect.enabled = true;
        endGameEffect = false;
    }

    public static void EndGameEffectGlitch()
    {
        glitchEffect.SetScan(true);
        endGameEffect = true;
    }

    private void Glitch(float duration, float amount)
    {
        if (duration >= 0.5f)
        {
            glitchEffect.enabled = true;
            if (!glitchEffect._xScan)
                Invoke("StopGlitch", duration*0.75f);
        }
    }

    private void StopGlitch()
    {
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