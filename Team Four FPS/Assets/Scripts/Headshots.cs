using System.Collections;
using System.Collections.Generic;
using TackleBox;
using UnityEngine;

public class Headshots : MonoBehaviour
{
    public GameObject parent;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (!this)
        {
            HeadShotDamage();
        }
        
    }
    public void HeadShotDamage()
    {
        EnemyAI temp = parent.GetComponent<EnemyAI>();
        int extraDamage = GameManager.Instance.PlayerScript.gunList[GameManager.Instance.PlayerScript.selectedGun].shootDamage;
        temp.takeDamage(extraDamage);
    }

}
