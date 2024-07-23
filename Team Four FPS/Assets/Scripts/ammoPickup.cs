using System.Collections;
using System.Collections.Generic;
using TackleBox;
using TackleBox.Audio; 
using TackleBox.Guns;
using UnityEngine;

public class ammoPickup : MonoBehaviour
{
    // Awake is called before the first frame update
    void Awake() 
    {
        playerController plrAmmo = GameManager.Instance.PlayerScript;
        AudioManager mgrSound = AudioManager.Instance;
        Audio AmmoDrop = mgrSound.GetSoundByID("AmmoDrop");
        if (GameManager.Instance.PlayerScript && GameManager.Instance.PlayerScript.plrAudio)
            AmmoDrop.PlayOneShot(GameManager.Instance.PlayerScript.plrAudio);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController playerAmmo = GameManager.Instance.PlayerScript;
            foreach(TackleBox.Guns.gunStats gun in playerAmmo.gunList)
            {
                gun.ammoCurr = gun.clipSize;
                gun.ammoMax = gun.ammoCapacity;
            }

            AudioManager soundManager = AudioManager.Instance;
            Audio AmmoBox = soundManager.GetSoundByID("AmmoBox");
            AmmoBox.PlayOneShot(GameManager.Instance.PlayerScript.plrAudio);

            playerAmmo.grenadeCount = 5;
            GameManager.Instance.grenadeCount = 5;
            playerAmmo.updatePlayerUI();
            GameManager.Instance.PlayerScript.updatePlayerUI();
            Destroy(gameObject);
        }
    }
}