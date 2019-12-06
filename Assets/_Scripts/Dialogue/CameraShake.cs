using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    CinemachineVirtualCamera virtualCamera;
    CinemachineBasicMultiChannelPerlin virtualCameraNoise;

    private void Start()
    {
        virtualCameraNoise = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }

    public void LoadCameraShake(float duration, float shakeSpeed, float intensity)
    {
        StartCoroutine(CorCameraShake(duration, shakeSpeed, intensity));
    }
    IEnumerator CorCameraShake(float duration, float shakeSpeed, float intensity)
    {
        print("shake");
        float startDuration = duration;
        float startIntensity = intensity;
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            float currentIntensity = (duration * startIntensity) / startDuration;
            virtualCameraNoise.m_AmplitudeGain = currentIntensity;
            
            virtualCameraNoise.m_FrequencyGain = shakeSpeed;
            yield return null;
        }
        virtualCameraNoise.m_AmplitudeGain = 0;
        virtualCameraNoise.m_FrequencyGain = 0;
    }
}
