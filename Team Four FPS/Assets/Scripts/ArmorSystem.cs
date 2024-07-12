using System.Collections;
using System.Collections.Generic;
using TackleBox;
using UnityEngine;

public class ArmorSystem : MonoBehaviour
{
    [Header("<=====COMPONENTS=====>")]

    [SerializeField] private GameObject placeArmor; 

    private int armorProtection;

    public void ArmorPack(int amount, GameObject armorSphere)
    {
        // Player Will Have A Full Armor Shield Without Stacking Multiply Green Armor Capsules

        armorProtection = amount;

        for (int num = placeArmor.transform.childCount - 1; num >= 0; num--) 
        {
            Destroy(placeArmor.transform.GetChild(num).gameObject); 
        }

        armorSphere.transform.position = transform.position; 
    }
    
}