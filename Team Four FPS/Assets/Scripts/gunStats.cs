using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    public GameObject gunModel;

    [Range(1, 10)] public int shootDamage;
    [Range(0.1f, 4f)] public float shootRate;
    [Range(10, 500)] public int shootDistance;
    public int ammoCurr;
    public int ammoMax;

    public ParticleSystem hitEffect;
    public AudioClip shootSound;
    [Range(0, 1)] public float audioVolume;
}
