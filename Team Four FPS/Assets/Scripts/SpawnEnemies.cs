using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TackleBox;
using JetBrains.Annotations;
using Unity.VisualScripting;

public class SpawnEnemies : MonoBehaviour
{
    [Header("<=====COMPONENTS=====>")]
    
    [SerializeField] GameObject spawnObjects;
    [SerializeField] Transform[] spawnPosition;
    

    [Header("<=====SPAWN_AUDIO=====>")]

    [SerializeField] AudioSource adSpawn; 

    [SerializeField] AudioClip[] spawnAudio;
    [SerializeField] float spawnVolume; 

    [Header("<=====ENEMY_SPAWN=====>")]

    [SerializeField] int spawnTimer;
    [SerializeField] int spawnNumber;
    
    
    [SerializeField] bool setSpawn;

    
    int numberSpawned = 0;

    bool spawnTruth;   
    bool spawnStart;  


    // Start is called before the first frame update
    void Start()
    {

        
        
        
    }
   
    // Update is called once per frame
    void Update()
    {
        if (setSpawn && spawnStart && numberSpawned < spawnNumber && !spawnTruth)
        {
            StartCoroutine(EnemySpawn());
        }
        else if(!setSpawn)
        {
            if (GameManager.Instance.enemyCount == 0)
            {
                GameManager.Instance.spawnMoreEnemies = true;
                
            }
            if (GameManager.Instance.spawnMoreEnemies && !spawnTruth && !GameManager.Instance.roundTransition && spawnStart && numberSpawned < spawnNumber)//spawnStart && spawnCount < spawnNumber && !spawnTruth) 
            {
                StartCoroutine(EnemySpawn());
                
                
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        { 
            spawnStart = true; 
        }
    }

    IEnumerator EnemySpawn() 
    {
        spawnTruth = true;

        int posArray = Random.Range(0, spawnPosition.Length);

        Instantiate(spawnObjects, spawnPosition[posArray].position, spawnPosition[posArray].rotation);

        numberSpawned++;
        
        GameManager.Instance.updateGameGoal(1);
        
        yield return new WaitForSeconds(spawnTimer);



        //adSpawn.PlayOneShot(spawnAudio[Random.Range(0, spawnAudio.Length)], spawnVolume);
        

        spawnTruth = false;

        if (numberSpawned >= spawnNumber && !setSpawn)
        {
            GameManager.Instance.spawnMoreEnemies = false;
            numberSpawned = 0;
            spawnNumber += 2;
           
            
                        
        }
       
        
        
        
    }
}