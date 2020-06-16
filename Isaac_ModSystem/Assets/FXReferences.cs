using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXReferences : MonoBehaviour
{
    public static FXReferences Instance = null;
    
    [Header("Tear FX")]
    public AudioClip[] tearShootClips;
    public AudioClip tearDestroyed;

    [Header("Player FX")]
    public AudioClip[] playerHurtFX;
    public AudioClip[] playerDeadFX;

    private void Awake()
    {
        Instance = this;
    }
}
