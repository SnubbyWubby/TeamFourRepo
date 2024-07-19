using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TackleBox;
using TackleBox.Guns;

public class gunPickup : MonoBehaviour
{
    [SerializeField] gunStats gun;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gun.ammoCurr = gun.clipSize;
            gun.ammoMax = gun.ammoCapacity;

            GameManager.Instance.PlayerScript.getGunStats(gun);
            Destroy(gameObject);
        }
    }
}
