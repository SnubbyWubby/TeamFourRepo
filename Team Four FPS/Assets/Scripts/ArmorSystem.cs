using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorSystem : MonoBehaviour
{
    [SerializeField] private GameObject placeArmor; 

    private int armorProtection;

    public void ArmorPack(int amount, GameObject armorSphere)
    {
        // Player Will Regenarate Full Armor When Walking Or Running Towards The Green Armor Sphere 

        armorProtection = amount;

        for (int num = placeArmor.transform.childCount - 1; num >= 0; num--) 
        {
            Destroy(placeArmor.transform.GetChild(num).gameObject); 
        }

        armorSphere.transform.SetParent(placeArmor.transform);
        armorSphere.transform.position = transform.position; 
    }
}