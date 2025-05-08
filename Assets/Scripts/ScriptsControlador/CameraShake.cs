using System.Collections;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    [Header("Cinemachine")]
    public CinemachineFreeLook freeLookCam;

    [Header("Shake Settings")]
    public float shakeDuration = 0.2f;
    public float shakeAmplitude = 2f;
    public float shakeFrequency = 2f;

    private CinemachineBasicMultiChannelPerlin[] noiseProfiles;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        // Obtener los noise profiles de los 3 rigs
        noiseProfiles = new CinemachineBasicMultiChannelPerlin[3];
        noiseProfiles[0] = freeLookCam.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noiseProfiles[1] = freeLookCam.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noiseProfiles[2] = freeLookCam.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera(float duration, float amplitude, float frequency)
    {
        StopAllCoroutines(); // Reinicia cualquier shake anterior
        StartCoroutine(ShakeCoroutine(duration, amplitude, frequency));
    }

    private IEnumerator ShakeCoroutine(float duration, float amplitude, float frequency)
    {
        foreach (var noise in noiseProfiles)
        {
            noise.m_AmplitudeGain = amplitude;
            noise.m_FrequencyGain = frequency;
        }

        yield return new WaitForSeconds(duration);

        foreach (var noise in noiseProfiles)
        {
            noise.m_AmplitudeGain = 0;
            noise.m_FrequencyGain = 0;
        }
    }
}
