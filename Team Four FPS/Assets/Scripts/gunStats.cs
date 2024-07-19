using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TackleBox.Guns
{
    [CreateAssetMenu(fileName = "Guns", menuName = "TackleBox.Guns/Gun Stats", order = 1)]
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
        public int clipSize;
        public float reloadTime;
        public int ammoCapacity;
        public float recoilAmount;

        [Header("<=====GUN_EFFECTS=====>")]

        public ParticleSystem hitEffect;
        public AudioClip shootSound;
    }
}