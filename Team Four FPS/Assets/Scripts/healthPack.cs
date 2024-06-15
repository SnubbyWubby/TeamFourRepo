using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthPack : MonoBehaviour
{
    playerController plrHealthPack;

    public int healthBoost; 

    void Awake()
    {
        plrHealthPack = FindObjectOfType<playerController>(); 
    }

    void OnTriggerEnter(Collider plrHealth)
    {
        if (plrHealth.CompareTag("Player"))
        {
            plrHealth.GetComponent<playerController>().HealthPack(healthBoost);

            Destroy(gameObject);

            Debug.Log("Restored-Health!"); 
        } 
    }
}