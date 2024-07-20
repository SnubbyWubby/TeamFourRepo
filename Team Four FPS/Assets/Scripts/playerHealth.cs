using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TackleBox;
using TackleBox.Audio;

public class playerHealth : MonoBehaviour
{
    [Header("<=====HEALTH_PICK_UP_AUDIO=====>")]

    public int hltAmount;   

    void OnTriggerEnter(Collider other)
    {
        // Allows The Player To Pick-Up A Full Health-Pack Blue Sphere 

        if (other.CompareTag("Player"))
        {
            other.GetComponent<playerController>().HealthPack(hltAmount);  

            Destroy(gameObject);
        }
    }
}
