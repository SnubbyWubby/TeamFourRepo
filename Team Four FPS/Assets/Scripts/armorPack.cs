using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class armorPack : MonoBehaviour
{
    [SerializeField] public int armorValue; 

    playerController plrArmorPack;

    public int armorBoost; 

    // Awake Function Is Called Right After The Start Function 
    void Awake()
    {
        plrArmorPack = FindObjectOfType<playerController>();
    }

    void OnTriggerEnter(Collider plrArmor)
    {
        // Allows The Player To Pick-Up A Full Armor-Pack Green Sphere 

        if (plrArmor.CompareTag("Player"))
        {
            plrArmor.GetComponent<playerController>().ArmorShield(armorBoost);  

            plrArmor.GetComponent<ArmorSystem>().ArmorPack(armorValue, gameObject); 

            Destroy(gameObject);  

            Debug.Log("Armor_Shield!"); 
        }
    }
}