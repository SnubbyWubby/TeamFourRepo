using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthPack : MonoBehaviour
{
    playerController plrHealthPack;

    public int healthBoost;

    // Awake Function Is Called Right After The Start Function 
    void Awake()
    {
        plrHealthPack = FindObjectOfType<playerController>(); 
    }

    void OnTriggerEnter(Collider plrHealth)
    {
        // Allows The Player To Pick-Up A Full Health-Pack Blue Sphere 

        if (plrHealth.CompareTag("Player"))
        {
            plrHealth.GetComponent<playerController>().HealthPack(healthBoost);

            Destroy(gameObject);

            Debug.Log("Health-Restored!"); 
        } 
    }
}