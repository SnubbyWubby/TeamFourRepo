using System.Collections;
using System.Collections.Generic;
using TackleBox;
using TackleBox.Audio;
using TackleBox.Guns;
using UnityEditor;
using UnityEngine;

public class healthPack : MonoBehaviour
{
    // Awake is called before the first frame update
    void Awake() 
    {
        playerController plrHealth = GameManager.Instance.PlayerScript;
        AudioManager mgrSound = AudioManager.Instance;
        Audio HealthDrop = mgrSound.GetSoundByID("HealthDrop");
        if (GameManager.Instance.PlayerScript && GameManager.Instance.PlayerScript.plrAudio)
            HealthDrop.PlayOneShot(GameManager.Instance.PlayerScript.plrAudio);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController toAdd = GameManager.Instance.PlayerScript;
            toAdd.HP = 10;
            GameManager.Instance.PlayerScript.updatePlayerUI();
            AudioManager soundManager = AudioManager.Instance;
            Audio hltRestore = soundManager.GetSoundByID("HealthPickUp");  
            hltRestore.PlayOneShot(GameManager.Instance.PlayerScript.plrAudio); 
            Destroy(gameObject);           
        } 
    }
}