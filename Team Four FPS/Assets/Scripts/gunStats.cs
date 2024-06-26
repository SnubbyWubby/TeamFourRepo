using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    [Header("<=====COMPONENTS=====>")]

    public GameObject gunModel;

    [Header("<=====PLAYER_GUN_STATS=====>")]

    [Range(1, 20)] public int shootDamage;
    [Range(10, 500)] public int shootDistance;

    [Range(0.1f, 3f)] public float shootRate;
    [Range(0, 1)] public float audioVolume;

    public int ammoCurr;
    public int ammoMax;

    [Header("<=====GUN_EFFECTS=====>")] 

    public ParticleSystem hitEffect;
    public AudioClip shootSound;
}
