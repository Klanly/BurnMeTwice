using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStep : MonoBehaviour
{
    [SerializeField]
    AudioClip[] clips;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void StepSoundEvent()
    {
        if (transform.parent.gameObject.GetComponent<PlayerMovement>() != null)
        {
            AudioClip stepClip = GetRandomClip();
            audioSource.PlayOneShot(stepClip);
        }
    }

    private AudioClip GetRandomClip()
    {
        return clips[UnityEngine.Random.Range(0, clips.Length - 1)];
    }
}
