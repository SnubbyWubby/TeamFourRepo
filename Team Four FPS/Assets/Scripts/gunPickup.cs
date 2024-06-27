using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunPickup : MonoBehaviour
{
    [SerializeField] gunStats gun;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gun.ammoCurr = gun.ammoMax; 

            GameManager.Instance.PlayerScript.getGunStats(gun);
            Destroy(gameObject);
        }
    }
}
