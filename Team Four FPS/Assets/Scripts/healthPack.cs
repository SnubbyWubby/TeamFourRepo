using System.Collections;
using System.Collections.Generic;
using TackleBox;
using TackleBox.Audio;
using TackleBox.Guns;
using UnityEditor;
using UnityEngine;

public class healthPack : MonoBehaviour
{
    

    

    
    

    // Awake Function Is Called Right After The Start Function 
    void Start()
    {
         
    }

    void OnTriggerEnter(Collider other)
    {
        // Allows The Player To Pick-Up A Full Health-Pack Blue Sphere 

        if (other.CompareTag("Player"))
        {
            AudioManager soundManager = AudioManager.Instance;
            Audio Weapon = soundManager.GetSoundByID("HealthPickUp");
            Weapon.PlayOneShot(GameManager.Instance.PlayerScript.plrAudio);
            playerController toAdd = GameManager.Instance.PlayerScript;
            toAdd.HP = 10;
            GameManager.Instance.PlayerScript.updatePlayerUI();
            Destroy(gameObject);

            
        } 
    }
}