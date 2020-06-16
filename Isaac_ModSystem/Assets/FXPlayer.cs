using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FXPlayer : MonoBehaviour
{
    public static FXPlayer Instance = null;
    private AudioSource audioSource;
    

    public AudioClip[] tearShootClips;
    private uint nextClipIndex = 0;

    public AudioClip tearDestroyed;

    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void TearShootFX()
    {
        if (audioSource.isPlaying)
            return;

        audioSource.clip = tearShootClips[nextClipIndex];
        nextClipIndex = nextClipIndex + 1 < tearShootClips.Length ? nextClipIndex + 1 : 0;    
    }

    public void TearDestroyed()
    {
        audioSource.clip = tearDestroyed;
        audioSource.Play();
    }
}
