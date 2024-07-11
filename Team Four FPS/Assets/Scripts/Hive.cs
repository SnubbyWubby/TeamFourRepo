using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hive : MonoBehaviour, IDamage
{
    [SerializeField] float HP;
    [SerializeField] GameObject spawner;
    [SerializeField] GameObject winSpawner;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        if (HP <= 0)
        {
            spawner.SetActive(true);
            winSpawner.SetActive(true);   
            Destroy(gameObject);
        }
    }


}
