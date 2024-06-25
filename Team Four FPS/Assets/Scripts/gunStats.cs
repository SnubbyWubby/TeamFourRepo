using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    public GameObject gunModel;

    [Range(1, 20)] public int shootDamage;
    [Range(10, 500)] public int shootDistance;

    [Range(0.1f, 3f)] public float shootRate;
    [Range(0, 1)] public float audioVolume;

    public int ammoCurr;
    public int ammoMax;

    public ParticleSystem hitEffect;
    public AudioClip shootSound;
}
