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

    [Header("Monsters")]
    public AudioClip squirtAttack;
    public AudioClip squirtDie;

    [Header("More Effects")]
    public AudioClip energyFull;
    public AudioClip roomCleared;
    public AudioClip[] itemPickup;

    private void Awake()
    {
        Instance = this;
    }
}
