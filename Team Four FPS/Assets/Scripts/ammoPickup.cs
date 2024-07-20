using System.Collections;
using System.Collections.Generic;
using TackleBox;
using TackleBox.Audio; 
using TackleBox.Guns;
using UnityEngine;

public class ammoPickup : MonoBehaviour
{
    [Header("<=====AMMO_PICK_UP_AUDIO=====>")]

    [SerializeField] AudioSource boxAudio;

    // Awake is called before the first frame update
    void Awake() 
    {
        playerController playerAmmo = GameManager.Instance.PlayerScript;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController playerAmmo = GameManager.Instance.PlayerScript;
            foreach(TackleBox.Guns.gunStats gun in playerAmmo.gunList)
            {
                boxAudio.Stop(); 
                gun.ammoCurr = gun.clipSize;
                gun.ammoMax = gun.ammoCapacity;
            }

            AudioManager soundManager = AudioManager.Instance;
            Audio Weapon = soundManager.GetSoundByID("HealthPickUp");
            Weapon.PlayOneShot(GameManager.Instance.PlayerScript.plrAudio);

            playerAmmo.grenadeCount = 5;
            GameManager.Instance.grenadeCount = 5;
            playerAmmo.updatePlayerUI();
            GameManager.Instance.PlayerScript.updatePlayerUI();
            Destroy(gameObject);
        }
    }
}