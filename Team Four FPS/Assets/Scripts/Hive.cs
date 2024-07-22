using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hive : MonoBehaviour
{
    public GameObject transferLevels;
    public GameObject particleEffects;

    private void OnTriggerEnter(Collider other)
    {
        transferLevels.SetActive(true); 
        particleEffects.SetActive(true);    
    }

}
