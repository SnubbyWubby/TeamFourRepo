using System.Collections;
using System.Collections.Generic;
using TackleBox;
using TackleBox.Guns;
using UnityEngine;

public class ammoPickup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController playerAmmo = GameManager.Instance.PlayerScript;
            foreach(TackleBox.Guns.gunStats gun in playerAmmo.gunList)
            {
                gun.ammoCurr = gun.ammoMax;
            }
            
            playerAmmo.grenadeCount = 5;
            GameManager.Instance.grenadeCount = 5;
            playerAmmo.updatePlayerUI();
            Destroy(gameObject);
        }
    }
}
